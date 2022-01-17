using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AmberXPay.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;



namespace AmberXPay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private string   Feedback = "AccountCreated('{0}','{1}','{2}');";

        private readonly AmberXPayContext amberxpaycontext;

        const string SessionUser = "";

        private static string GenerateWalletID()
        {
            Random generator = new Random();
            String r = generator.Next(0, 100000000).ToString("D7");
            if (r.Distinct().Count() == 1)
            {
                r = GenerateWalletID();
            }
            return r;
        }


        public HomeController(ILogger<HomeController> logger, AmberXPayContext context)
        {
            _logger = logger;

            amberxpaycontext = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult WebApp()
        {
            return View();
        }

        public IActionResult Login()
        {
            HttpContext.Session.Clear();

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult App()
        {

            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionUser)))
            {
                HttpContext.Session.Clear();
            
                return View("~/Views/Home/Login.cshtml");
            }

           return View(); 
        }
        
        public IActionResult Settings()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Register(UserData data)
        {
            var Member = new AmberXPayModel();

            bool CheckEmail    = amberxpaycontext.Axp_User_Details.Where(c => c.Email == data.Email).Any();
            bool CheckUserMobile = amberxpaycontext.Axp_User_Details.Where(c => c.Mobile_Number == data.Mobile_Number).Any();

            if ((CheckEmail != true) & (CheckUserMobile != true))
            {


                Member.FirstName     = data.FirstName;
                Member.LastName      = data.LastName;
                Member.Mobile_Number = data.Mobile_Number;
                Member.Date_of_Birth = data.Date_of_Birth;
                Member.Email         = data.Email;
                Member.Country_Name  = data.Country_Code;
                Member.Password      = data.Password;
                Member.Status        = "pending";
                Member.Create_Date   = DateTime.Now;

                amberxpaycontext.Add(Member);
                amberxpaycontext.SaveChanges();

               ViewBag.ErrorMessage = string.Format(  Feedback, "Congratulation!", "Your account has been created", "success");

            }
            else if (CheckEmail)
            {
                ViewBag.ErrorMessage = string.Format(  Feedback, "Oops!", "This email already exists", "warning");

                return View("~/Views/Home/Register.cshtml");
            }
            else if (CheckUserMobile)
            {
                ViewBag.ErrorMessage = string.Format(  Feedback, "Oops!", "The mobile number provided is already linked to an existing account.", "warning");

                return View("~/Views/Home/Register.cshtml");
            }


            return View("~/Views/Home/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(UserData data)
        {
            HttpContext.Session.SetString(SessionUser, " "); ;

            var Member = new AmberXPayModel();

            var entry = amberxpaycontext.Axp_User_Details.Where(c => c.Email.Equals(data.Email) && c.Password.Equals(data.Password)).FirstOrDefault();


            if (entry != null)
            {
                var User = entry.FirstName + " " + entry.LastName;

                HttpContext.Session.SetString(SessionUser, User);
                
                ViewBag.User = HttpContext.Session.GetString(SessionUser);

                return View("~/Views/Home/App.cshtml", entry);
            }
            else
            {
                ViewBag.ErrorMessage = string.Format(  Feedback, "Oops!", "invalid data provided", "warning");
            }


            return View("~/Views/Home/Login.cshtml");
        }

        public IActionResult GetWallets([FromBody]UserData data)
        {
            List<WalletModel> Wallets = new List<WalletModel>();

            var Wallet = new WalletModel();

            var GetWallets = amberxpaycontext.Axp_Wallet.Where(c => c.Email.Equals(data.Email)).ToList();


            return Json(new { success = true, Wallets = GetWallets });

        }

        [HttpPost]
        public IActionResult CreateWallet ([FromBody]UserData data)
        {
            var WalletID = GenerateWalletID();

            var DbResponse = "Y";
            var Wallet = new WalletModel();
            List<Response> CreationResponse = new List<Response>();

            if (data.WalletCurrency == "NGN")
            {
                bool CheckEmail = amberxpaycontext.Axp_Wallet.Where(c => c.Email == data.Email).Any();
                bool CheckWalletId = amberxpaycontext.Axp_Wallet.Where(c => c.NGN_Wallet_ID == "611" + WalletID).Any();
                var CheckWallet = amberxpaycontext.Axp_Wallet.Where(c => c.Email.Equals(data.Email) && string.IsNullOrEmpty(c.NGN_Wallet_ID)).FirstOrDefault();


                if (CheckEmail != true && CheckWalletId != true)
                {

                    Wallet.Email = data.Email;
                    Wallet.NGN_Wallet_ID = "611"+  WalletID;
                    Wallet.NGN_Wallet_Balance = "0.00";
                    Wallet.NGN_Wallet_Create_Date = DateTime.Now;


                    amberxpaycontext.Add(Wallet);
                    amberxpaycontext.SaveChanges();
                    DbResponse = "Y";
                }

                else  if (CheckEmail == true && CheckWalletId != true)
                {
                    if (CheckWallet != null)
                    {
                        CheckWallet.Email = data.Email;
                        CheckWallet.NGN_Wallet_ID = "611" + WalletID;
                        CheckWallet.NGN_Wallet_Balance = "0.00";
                        CheckWallet.NGN_Wallet_Create_Date = DateTime.Now;


                        amberxpaycontext.SaveChanges();
                        DbResponse = "Y";
                    }
                    else if (CheckWallet == null) { DbResponse = "E"; }

                } 
            }   
            if (data.WalletCurrency == "USD")
            {
                bool CheckEmail = amberxpaycontext.Axp_Wallet.Where(c => c.Email == data.Email).Any();
                bool CheckWalletId = amberxpaycontext.Axp_Wallet.Where(c => c.USD_Wallet_ID == "612" + WalletID).Any();
                var CheckWallet = amberxpaycontext.Axp_Wallet.Where(c => c.Email.Equals(data.Email) && string.IsNullOrEmpty(c.USD_Wallet_ID)).FirstOrDefault();

                if(CheckEmail != true && CheckWalletId != true)
                {

                    Wallet.Email = data.Email;
                    Wallet.USD_Wallet_ID = "612" + WalletID;
                    Wallet.USD_Wallet_Balance = "0.00";
                    Wallet.USD_Wallet_Create_Date = DateTime.Now;


                    amberxpaycontext.Add(Wallet);
                    amberxpaycontext.SaveChanges();
                    DbResponse = "Y";
                }

                else if (CheckEmail == true && CheckWalletId != true)
                {
                    if (CheckWallet != null)
                    {
                        CheckWallet.Email = data.Email;
                        CheckWallet.USD_Wallet_ID = "612" + WalletID;
                        CheckWallet.USD_Wallet_Balance = "0.00";
                        CheckWallet.USD_Wallet_Create_Date = DateTime.Now;


                        amberxpaycontext.SaveChanges();
                        DbResponse = "Y";
                    }
                    else if (CheckWallet == null) { DbResponse = "E"; }

                }

            }
            if (data.WalletCurrency == "CAD")
            {
                bool CheckEmail = amberxpaycontext.Axp_Wallet.Where(c => c.Email == data.Email).Any();
                bool CheckWalletId = amberxpaycontext.Axp_Wallet.Where(c => c.CAD_Wallet_ID == "613" + WalletID).Any();
                var CheckWallet = amberxpaycontext.Axp_Wallet.Where(c => c.Email.Equals(data.Email) && string.IsNullOrEmpty(c.CAD_Wallet_ID)).FirstOrDefault();

                if (CheckEmail != true && CheckWalletId != true)
                {

                    Wallet.Email = data.Email;
                    Wallet.CAD_Wallet_ID = "613" + WalletID;
                    Wallet.CAD_Wallet_Balance = "0.00";
                    Wallet.CAD_Wallet_Create_Date = DateTime.Now;


                    amberxpaycontext.Add(Wallet);
                    amberxpaycontext.SaveChanges();
                    DbResponse = "Y";
                }

                else if (CheckEmail == true && CheckWalletId != true)
                {
                    if (CheckWallet != null)
                    {
                        CheckWallet.Email = data.Email;
                        CheckWallet.CAD_Wallet_ID = "613" + WalletID;
                        CheckWallet.CAD_Wallet_Balance = "0.00";
                        CheckWallet.CAD_Wallet_Create_Date = DateTime.Now;


                        amberxpaycontext.SaveChanges();
                        DbResponse = "Y";
                    }
                    else if (CheckWallet == null) { DbResponse = "E"; }

                }


            }
            if (data.WalletCurrency == "GBP")
            {
                bool CheckEmail = amberxpaycontext.Axp_Wallet.Where(c => c.Email == data.Email).Any();
                bool CheckWalletId = amberxpaycontext.Axp_Wallet.Where(c => c.GBP_Wallet_ID == "614" + WalletID).Any();
                var CheckWallet = amberxpaycontext.Axp_Wallet.Where(c => c.Email.Equals(data.Email) && string.IsNullOrEmpty(c.GBP_Wallet_ID)).FirstOrDefault();

                if (CheckEmail != true && CheckWalletId != true)
                {

                    Wallet.Email = data.Email;
                    Wallet.GBP_Wallet_ID = "614" + WalletID;
                    Wallet.GBP_Wallet_Balance = "0.00";
                    Wallet.GBP_Wallet_Create_Date = DateTime.Now;


                    amberxpaycontext.Add(Wallet);
                    amberxpaycontext.SaveChanges();
                    DbResponse = "Y";
                }

                else if (CheckEmail == true && CheckWalletId != true)
                {
                    if (CheckWallet != null)
                    {
                        CheckWallet.Email = data.Email;
                        CheckWallet.GBP_Wallet_ID = "614" + WalletID;
                        CheckWallet.GBP_Wallet_Balance = "0.00";
                        CheckWallet.GBP_Wallet_Create_Date = DateTime.Now;


                        amberxpaycontext.SaveChanges();
                        DbResponse = "Y";
                    }
                    else if (CheckWallet == null) { DbResponse = "E"; }

                }

            }
            if (data.WalletCurrency == "EURO")
            {
                bool CheckEmail = amberxpaycontext.Axp_Wallet.Where(c => c.Email == data.Email).Any();
                bool CheckWalletId = amberxpaycontext.Axp_Wallet.Where(c => c.EURO_Wallet_ID == "615" + WalletID).Any();
                var CheckWallet = amberxpaycontext.Axp_Wallet.Where(c => c.Email.Equals(data.Email) && string.IsNullOrEmpty(c.EURO_Wallet_ID)).FirstOrDefault();

                if (CheckEmail != true && CheckWalletId != true)
                {

                    Wallet.Email = data.Email;
                    Wallet.EURO_Wallet_ID = "615" + WalletID;
                    Wallet.EURO_Wallet_Balance = "0.00";
                    Wallet.EURO_Wallet_Create_Date = DateTime.Now;


                    amberxpaycontext.Add(Wallet);
                    amberxpaycontext.SaveChanges();
                    DbResponse = "Y";
                }

                else if (CheckEmail == true && CheckWalletId != true)
                {
                    if (CheckWallet != null)
                    {
                        CheckWallet.Email = data.Email;
                        CheckWallet.EURO_Wallet_ID = "615" + WalletID;
                        CheckWallet.EURO_Wallet_Balance = "0.00";
                        CheckWallet.EURO_Wallet_Create_Date = DateTime.Now;


                        amberxpaycontext.SaveChanges();
                        DbResponse = "Y";
                    }
                    else if (CheckWallet == null) { DbResponse = "E"; }

                }

            }


            return Json(new { success = true, CreationResponse = DbResponse });

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
