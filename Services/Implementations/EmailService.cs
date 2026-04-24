using HotelBooking.Services.Interfaces;
using HotelBooking.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace HotelBooking.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendBookingConfirmationAsync(
        string toEmail,
        int bookingId,
        string hotelName,
        string roomNumber,
        DateTime checkIn,
        DateTime checkOut,
        decimal totalPrice)
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(_config["EmailSettings:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Booking Confirmation";

        email.Body = new TextPart("html")
        {
            Text = $@"
            <div style='background:#0a0a0f;padding:40px 0;font-family:Arial,sans-serif;color:#f0ece0;'>
                
                <div style='max-width:600px;margin:auto;background:#111118;border:1px solid rgba(201,168,76,0.2);border-radius:12px;overflow:hidden;'>
                    
                    <!-- Header -->
                    <div style='padding:30px;text-align:center;border-bottom:1px solid rgba(201,168,76,0.15);'>
                        <h1 style='margin:0;font-size:28px;font-weight:300;color:#c9a84c;letter-spacing:2px;text-transform:uppercase;'>
    Booking Confirmed
</h1>
                        <p style='margin-top:10px;color:#7a7670;font-size:13px;letter-spacing:2px;text-transform:uppercase;'>
                            Luxury Stay Experience
                        </p>
                    </div>

                    <!-- Content -->
                    <div style='padding:30px;'>

                        <p style='font-size:15px;margin-bottom:20px;'>
                            Your booking has been successfully confirmed.
                        </p>

                        <div style='background:#16161f;border:1px solid rgba(201,168,76,0.15);border-radius:10px;padding:20px;margin-bottom:25px;'>

                            <p><strong style='color:#c9a84c;'>Booking ID:</strong> {bookingId}</p>
                            <p><strong style='color:#c9a84c;'>Hotel:</strong> {hotelName}</p>
                            <p><strong style='color:#c9a84c;'>Room:</strong> {roomNumber}</p>
                            <p><strong style='color:#c9a84c;'>Check-in:</strong> {checkIn:d}</p>
                            <p><strong style='color:#c9a84c;'>Check-out:</strong> {checkOut:d}</p>

                            <hr style='border:none;border-top:1px solid rgba(201,168,76,0.15);margin:15px 0;'>

                            <p style='font-size:16px;'>
                                <strong style='color:#e8c97a;'>Total Paid:</strong> ${totalPrice}
                            </p>
                        </div>

                        <!-- CTA Button -->
                        <div style='text-align:center;margin-top:20px;'>
                            <a href='http://127.0.0.1:4200/my-bookings'
                               style='display:inline-block;padding:12px 28px;
                                      background:linear-gradient(135deg,#c9a84c,#e8c97a);
                                      color:#0a0a0f;
                                      text-decoration:none;
                                      font-size:12px;
                                      letter-spacing:1px;
                                      border-radius:4px;
                                      font-weight:600;'>
                                VIEW BOOKING
                            </a>
                        </div>

                    </div>

                    <!-- Footer -->
                    <div style='padding:20px;text-align:center;border-top:1px solid rgba(201,168,76,0.1);font-size:12px;color:#7a7670;'>
                        Thank you for choosing our service
                    </div>

                </div>

            </div>
            "
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _config["EmailSettings:SmtpServer"],
            int.Parse(_config["EmailSettings:Port"]),
            MailKit.Security.SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _config["EmailSettings:SenderEmail"],
            _config["EmailSettings:SenderPassword"]
        );

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendBookingCancellationAsync(string toEmail, int bookingId)
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(_config["EmailSettings:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Booking Cancelled";

        email.Body = new TextPart("html")
        {
            Text = $@"
            <div style='background:#0a0a0f;padding:40px 0;font-family:Arial,sans-serif;color:#f0ece0;'>
                <div style='max-width:600px;margin:auto;background:#111118;border-radius:12px;padding:30px;text-align:center;'>
                    
                    <h1 style='color:#ff6b6b;'>Booking Cancelled</h1>
                    <p>Your booking #{bookingId} has been cancelled.</p>

                </div>
            </div>
            "
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _config["EmailSettings:SmtpServer"],
            int.Parse(_config["EmailSettings:Port"]),
            MailKit.Security.SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _config["EmailSettings:SenderEmail"],
            _config["EmailSettings:SenderPassword"]
        );

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}