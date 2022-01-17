using System;
namespace AmberXPay.Models
{
    public class UserData
    {
        public int User_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile_Number { get; set; }
        public DateTime Date_of_Birth { get; set; }
        public string Mobile_Verification { get; set; }
        public string Country_Name { get; set; }
        public string Country_Code { get; set; }
        public string Avatar { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string WalletCurrency { get; set; }
        public DateTime Create_Date { get; set; }
        public string Response { get; set; }

    }
}
