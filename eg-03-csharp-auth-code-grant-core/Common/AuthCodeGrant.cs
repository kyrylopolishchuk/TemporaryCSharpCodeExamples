using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DocuSign.eSignature
{
	public static class DocuSignWebAppExtensions
	{
		public static void ConfigureDS(this IApplicationBuilder applicationBuilder)
		{
			applicationBuilder.UseAuthentication();
			applicationBuilder.UseSession();
		}

		public static IMvcBuilder ConfigureMVCForDS(this IMvcBuilder mvcBuilder) =>
			mvcBuilder.AddMvcOptions(options => options.Filters.Add<LocalsFilter>());

		public static void ConfigureDS(this IServiceCollection services, IConfiguration Configuration)
		{
			DSConfiguration config = new DSConfiguration();

			Configuration.Bind("DocuSign", config);
			services.AddSingleton(config);
			services.AddScoped<IRequestItemsService, RequestItemsService>();

			services.AddMemoryCache();
			services.AddSession();
			services.AddHttpContextAccessor();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddAuthentication(options =>
			{
				options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = "DocuSign";
			})
			.AddCookie()
			.AddOAuth("DocuSign", options =>
			{
				options.ClientId = Configuration["DocuSign:IntegrationKey"];
				options.ClientSecret = Configuration["DocuSign:SecretKey"];
				options.CallbackPath = new PathString("/ds/callback");

				options.AuthorizationEndpoint = Configuration["DocuSign:AuthorizationEndpoint"];
				options.TokenEndpoint = Configuration["DocuSign:TokenEndpoint"];
				options.UserInformationEndpoint = Configuration["DocuSign:UserInformationEndpoint"];
				options.Scope.Add("signature");
				options.SaveTokens = true;
				options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
				options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
				options.ClaimActions.MapJsonKey("accounts", "accounts");

				options.ClaimActions.MapJsonKey("access_token", "access_token");
				options.ClaimActions.MapJsonKey("refresh_token", "refresh_token");
				options.ClaimActions.MapJsonKey("expires_in", "expires_in");
				options.Events = new OAuthEvents
				{
					OnCreatingTicket = async context =>
					{
						var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
						request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

						var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
						response.EnsureSuccessStatusCode();
						var responce = JObject.Parse(await response.Content.ReadAsStringAsync());
						var ris = context.HttpContext.RequestServices.GetService(typeof(IRequestItemsService)) as RequestItemsService;
						var session = new Session
						{
							AccountId = responce["accounts"][0]["account_id"].ToString(),
							AccountName = responce["accounts"][0]["account_name"].ToString(),
							BasePath = responce["accounts"][0]["base_uri"].ToString()
						};
						var user = new User
						{
							AccessToken = context.AccessToken,
							RefreshToken = context.RefreshToken,
							ExpireIn = DateTime.Now.Add(context.ExpiresIn.Value),
							Name = responce["name"].ToString()
						};
						ris.UpdateValues(user, session, context.HttpContext.User.Identity.Name);
						context.RunClaimActions();
					},
					OnRemoteFailure = context =>
					{
						context.HandleResponse();
						context.Response.Redirect("/Error?message=" + context.Failure.Message);
						return Task.FromResult(0);
					}
				};
			});
		}
	}

	public static class SessionExtensions
	{
		public static void SetObjectAsJson(this ISession session, string key, object value)
		{
			session.SetString(key, JsonConvert.SerializeObject(value));
		}

		public static T GetObjectFromJson<T>(this ISession session, string key)
		{
			var value = session.GetString(key);

			return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
		}
	}

	public class Locals
	{
		public DSConfiguration DsConfig { get; set; }
		public User User { get; set; }
		public Session Session { get; set; }
		public String Messages { get; set; }
		public object Json { get; internal set; }
	}

	public class LocalsFilter : IActionFilter
	{
		DSConfiguration Config { get; }

		private readonly IRequestItemsService _requestItemsService;
		private IMemoryCache _cache;

		public LocalsFilter(DSConfiguration config, IRequestItemsService requestItemsService, IMemoryCache cache)
		{
			Config = config;
			_cache = cache;
			_requestItemsService = requestItemsService;
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{

		}

		public void OnActionExecuting(ActionExecutingContext context)
		{

			Controller controller = context.Controller as Controller;

			if (controller == null)
			{
				return;
			}
			var viewBag = controller.ViewBag;
			var httpContext = context.HttpContext;

			var locals = httpContext.Session.GetObjectFromJson<Locals>("locals") ?? new Locals();
			locals.DsConfig = Config;
			locals.Session = httpContext.Session.GetObjectFromJson<Session>("session") ?? null;
			locals.Messages = "";
			locals.Json = null;
			locals.User = null;
			viewBag.Locals = locals;
			viewBag.showDoc = Config.documentation != null;


			var identity = httpContext.User.Identity as ClaimsIdentity;
			if (identity != null && !identity.IsAuthenticated)
			{
				locals.Session = new Session();
				return;
			}


			locals.User = httpContext.Session.GetObjectFromJson<User>("user");

			if (locals.User == null)
			{
				locals.User = _requestItemsService.User;
			}
			if (locals.Session == null)
			{
				locals.Session = _requestItemsService.Session;
			}
		}
	}

	public class DSConfiguration
	{
		public string AppUrl { get; set; }

		public string SignerEmail { get; set; }

		public string SignerName { get; set; }

		public string GatewayAccountId { get; set; }

		public string GatewayName { get; set; }

		public string GatewayDisplayName { get; set; }

		public bool production = false;
		public bool debug = true; // Send debugging statements to console
		public string sessionSecret = "12345"; // Secret for encrypting session cookie content
		public bool allowSilentAuthentication = true; // a user can be silently authenticated if they have an
													  // active login session on another tab of the same browser
													  // Set if you want a specific DocuSign AccountId, If null, the user's default account will be used.
		public string targetAccountId = null;
		public string demoDocPath = "demo_documents";
		public string docDocx = "World_Wide_Corp_Battle_Plan_Trafalgar.docx";
		public string tabsDocx = "World_Wide_Corp_salary.docx";
		public string docPdf = "World_Wide_Corp_lorem.pdf";
		public string githubExampleUrl = "https://github.com/docusign/eg-03-csharp-auth-code-grant-core/tree/master/eg-03-csharp-auth-code-grant-core/Controllers/";
		public string documentation = null;
	}

	public class Session
	{
		public string AccountId { get; set; }
		public string AccountName { get; set; }
		public string BasePath { get; set; }
	}

	public class User
	{
		public string Name { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public DateTime? ExpireIn { get; set; }
	}

	public interface IRequestItemsService
	{
		Session Session { get; set; }

		User User { get; set; }
	}

	public class RequestItemsService : IRequestItemsService
	{
		private readonly IMemoryCache _cache;
		private string _id;
		private string _accessToken;

		public RequestItemsService(IMemoryCache cache)
		{
			_cache = cache;
		}

		public void UpdateValues(User user, Session session, string id)
		{
			_id = id;
			_accessToken = user.AccessToken;
			User = user;
			Session = session;
		}

		private string GetKey(string key)
		{
			return string.Format("{0}_{1}", _id, key);
		}

		public Session Session
		{
			get => _cache.Get<Session>(GetKey("Session"));
			set => _cache.Set(GetKey("Session"), value);
		}

		public User User
		{
			get => _cache.Get<User>(GetKey("User"));
			set => _cache.Set(GetKey("User"), value);
		}


	}
}
