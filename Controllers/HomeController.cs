using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tenant.Models;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Tenant.Controllers
{
    public class HomeController : Controller       
    {
        public string RecatpchaSecretKey
        { //"6Ld8MGYUAAAAAAlp3bIMt1JpGtgXgD-hjWQTXWC_"           
            get
            {
                return _configuration["Captcha:SecretKey:Value"]; 
            }
        }

        public string RecaptchaSiteKey
        {//"6Ld8MGYUAAAAAF-agxDENNPPNukwYmf6q3Bsgp_M"
            get
            {
                return _configuration["Captcha:SiteKey:Value"];
            }
        }

        public string FromEmail
        {
            get
            {
                return _configuration["Email:From:Value"];
            }
        }

        public string ToEmailMu
        {
            get
            {
                return _configuration["Email:ToMU:Value"];
            }
        }

        public string ToEmailKou
        {
            get
            {
                return _configuration["Email:ToKou:Value"];
            }
        }

        public string CcEmail
        {
            get
            {
                return _configuration["Email:Cc:Value"];
            }
        }

        public string EmailPwd
        {
            get
            {
                return _configuration["Email:Password:Value"];
            }
        }



        public HomeController(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        private IConfiguration _configuration;
        //private readonly ITest _test;
        [HttpGet]
        public IActionResult Index()
        {
            var model = new ContactFormModel();
            model.RecapSiteKey = RecaptchaSiteKey;// "6Ld8MGYUAAAAAF-agxDENNPPNukwYmf6q3Bsgp_M";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactFormModel vm)
        {
                    
            var captchaResponse = await ValidateRecaptcha(Request, RecatpchaSecretKey);
            if (!captchaResponse.Success)
            {
                ModelState.AddModelError("recaptchaerror", "reCAPTCHA Error occured. Please try again");
                ViewBag.Message = "reCAPTCHA Error occured. Please try again";
                ViewBag.Section = "contact";
                var model = new ContactFormModel
                {
                    RecapSiteKey = RecaptchaSiteKey
                };
                return View(model);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    MailMessage msz = new MailMessage
                    {
                        From = new MailAddress(FromEmail)//Email which you are getting 
                    };
                    //from contact us page 
                    msz.To.Add(ToEmailMu);//Where mail will be sent 
                    msz.CC.Add(CcEmail);//Where mail will be sent 
                    msz.Subject = "MU ContactUs: "+ vm.Subject;
                    msz.Body = "From: " + vm.Name + Environment.NewLine + "Email: " + vm.Email + Environment.NewLine + "Message: " + Environment.NewLine + vm.Message;
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "mail.kouestates.org",
                        Credentials = new System.Net.NetworkCredential(FromEmail, EmailPwd)
                    };


                    smtp.Send(msz);

                    ModelState.Clear();
                    ViewBag.Section = "contact"; 
                    ViewBag.Message = "Mahalo for Contacting us!  Your email has been sent. We will be in touch!";
                }
                catch (Exception ex)
                {
                    ModelState.Clear();
                    ViewBag.Message = $" Sorry we are having an issue sending your.  Try again at a later time.";
                }
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Privacy()
        {
            const string V = "MU Privacy Policy";
            ViewData["Message"] = V;

            return View();
        }

        public IActionResult Contact()
        {
           // ViewData["Message"] = "Your contact page.";

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Contact(ContactFormModel vm)
        {

          
            // var recpatchaSecretKey = "6Ld8MGYUAAAAAAlp3bIMt1JpGtgXgD-hjWQTXWC_";
            var captchaResponse = await ValidateRecaptcha(Request, RecatpchaSecretKey);
            if (!captchaResponse.Success)
            {
                ModelState.AddModelError("recaptchaerror", "reCAPTCHA Error occured. Please try again");
                ViewBag.Message = "reCAPTCHA Error occured. Please try again";
                ViewBag.Section = "contact";
                var model = new ContactFormModel();
                model.RecapSiteKey = RecaptchaSiteKey;
                return View(model);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    MailMessage msz = new MailMessage
                    {
                        From = new MailAddress(FromEmail)//Email which you are getting 
                    };
                    //from contact us page 
                    msz.To.Add(ToEmailMu);//Where mail will be sent 
                    msz.CC.Add(CcEmail);//Where mail will be sent 
                    msz.Subject = "MU ContactUs: " + vm.Subject;
                    msz.Body = "From: " + vm.Name + Environment.NewLine + "Email: " + vm.Email + Environment.NewLine + "Message: " + Environment.NewLine + vm.Message;
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "mail.kouestates.org",
                        Credentials = new System.Net.NetworkCredential(FromEmail, EmailPwd)
                    };


                    smtp.Send(msz);

                    ModelState.Clear();
                    ViewBag.Section = "contact";
                    ViewBag.Message = "Your email has been sent and we will be in touch!";
                }
                catch (Exception ex)
                {
                    ModelState.Clear();
                    ViewBag.Message = $" Sorry we are having an issue sending your.  Try again at a later time.";
                }
            }

            return View();
          
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<RecaptchaResponse> ValidateRecaptcha(
           HttpRequest request,
           string secretKey)
        {
            var response = request.Form["g-recaptcha-response"];
            var client = new HttpClient();
            string result = await client.GetStringAsync(
                string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",
                    secretKey,
                    response)
                    );

            var captchaResponse = JsonConvert.DeserializeObject<RecaptchaResponse>(result);

            return captchaResponse;
        }
    }

    
}
