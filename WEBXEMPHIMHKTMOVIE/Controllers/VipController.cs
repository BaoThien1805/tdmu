using System;
using System.Configuration;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;
using WEBXEMPHIMHKTMOVIE.Helpers; // PayLib + Util

namespace WEBXEMPHIMHKTMOVIE.Controllers
{
    public class VipController : Controller
    {
        private readonly MovieWebDbContext db = new MovieWebDbContext();

        // =========================
        // TRANG VIP
        // =========================
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            ViewBag.IsVip = Session["IsVip"];
            ViewBag.VipExpiredAt = Session["VipExpiredAt"];

            return View();
        }

        public ActionResult CreatePayment()
        {
            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode",
                ConfigurationManager.AppSettings["vnp_TmnCode"]);
            pay.AddRequestData("vnp_Amount", (50000 * 100).ToString());
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan VIP test");
            pay.AddRequestData("vnp_OrderType", "billpayment");
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_ReturnUrl",
                ConfigurationManager.AppSettings["vnp_ReturnUrl"]);
            pay.AddRequestData("vnp_CreateDate",
                DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);

            string url = pay.CreateRequestUrl(
                ConfigurationManager.AppSettings["vnp_Url"],
                ConfigurationManager.AppSettings["vnp_HashSecret"]
            );

            return Redirect(url);
        }

        public ActionResult PaymentReturn()
        {
            PayLib pay = new PayLib();

            // CHỈ LẤY CÁC KEY BẮT ĐẦU = "vnp_"
            foreach (string key in Request.QueryString)
            {
                if (key.StartsWith("vnp_"))
                    pay.AddResponseData(key, Request.QueryString[key]);
            }

            string vnpSecureHash = Request.QueryString["vnp_SecureHash"];
            string hashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            bool valid = pay.ValidateSignature(vnpSecureHash, hashSecret);

            string responseCode = pay.GetResponseData("vnp_ResponseCode");
            string status = pay.GetResponseData("vnp_TransactionStatus");

            if (!valid)
            {
                ViewBag.Message = "❌ Sai chữ ký VNPay – giao dịch không hợp lệ!";
                return View();
            }

            if (responseCode == "00" && status == "00")
            {
                // ===== UPDATE VIP =====
                int userId = (int)Session["UserId"];
                var user = db.Users.Find(userId);

                DateTime now = DateTime.Now;

                user.IsVip = true;
                user.VipExpiredAt =
                    (user.VipExpiredAt != null && user.VipExpiredAt > now)
                    ? user.VipExpiredAt.Value.AddMonths(1)
                    : now.AddMonths(1);

                db.SaveChanges();

                Session["IsVip"] = true;
                Session["VipExpiredAt"] = user.VipExpiredAt;

                // ===== LẤY THÔNG TIN GIAO DỊCH =====
                ViewBag.Message = "✅ Thanh toán thành công – VIP đã kích hoạt!";
                ViewBag.OrderId = pay.GetResponseData("vnp_TxnRef");
                ViewBag.Amount = pay.GetResponseData("vnp_Amount");
                ViewBag.Time = pay.GetResponseData("vnp_PayDate");
                ViewBag.Bank = pay.GetResponseData("vnp_BankCode");
            }
            else
            {
                ViewBag.Message = "❌ Thanh toán thất bại hoặc bị huỷ!";
            }

            return View();
        }


    }
}
