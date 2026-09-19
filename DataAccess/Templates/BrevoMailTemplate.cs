using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DataAccess.Templates
{
    public class BrevoMailTemplate
    {
        public string BuildVisitorAlertHtml(string city, string country, string time, string browser, string os)
        {
            var location = $"{Encode(city)}, {Encode(country)}";
            DateTime dt = DateTime.Parse("2026-09-19 21:21:11");
            string formattedTime = dt.ToString("dd MMMM, yyyy hh:mm tt");
            // Output: "19 September, 2026 09:21 PM"


            return $"""
            <!DOCTYPE html>
            <html>
            <head>
              <meta charset="UTF-8"/>
              <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
            </head>
            <body style="margin:0;padding:0;background:#f4f4f5;font-family:Arial,sans-serif;">

              <table width="100%" cellpadding="0" cellspacing="0" style="padding:40px 0;">
                <tr>
                  <td align="center">
                    <table width="100%" cellpadding="0" cellspacing="0" style="max-width:520px;background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 2px 12px rgba(0,0,0,0.08);">

                      <!-- HEADER -->
                      <tr>
                        <td style="background:#0f0f0f;padding:32px 40px;text-align:center;">
                          <p style="margin:0;font-size:28px;">👀</p>
                          <h1 style="margin:10px 0 4px;color:#ffffff;font-size:20px;font-weight:600;letter-spacing:0.5px;">
                            Someone visited your Portfolio
                          </h1>
                          <p style="margin:0;color:#888888;font-size:13px;">Real-time visitor alert</p>
                        </td>
                      </tr>

                      <!-- BODY -->
                      <tr>
                        <td style="padding:36px 40px;">

                          <!-- Location Row -->
                          <table width="100%" cellpadding="0" cellspacing="0" style="margin-bottom:16px;">
                            <tr>
                              <td style="background:#f9f9f9;border-radius:8px;padding:16px 20px;">
                                <p style="margin:0 0 4px;font-size:11px;color:#999999;text-transform:uppercase;letter-spacing:1px;">Location</p>
                                <p style="margin:0;font-size:18px;font-weight:600;color:#0f0f0f;">📍 {location}</p>
                              </td>
                            </tr>
                          </table>

                          <!-- Time Row -->
                          <table width="100%" cellpadding="0" cellspacing="0" style="margin-bottom:16px;">
                            <tr>
                              <td style="background:#f9f9f9;border-radius:8px;padding:16px 20px;">
                                <p style="margin:0 0 4px;font-size:11px;color:#999999;text-transform:uppercase;letter-spacing:1px;">Visit Time</p>
                                <p style="margin:0;font-size:15px;font-weight:500;color:#0f0f0f;">🕐 {Encode(formattedTime)}</p>
                              </td>
                            </tr>
                          </table>

                          <!-- Browser + OS side by side -->
                          <table width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                              <td width="48%" valign="top" style="background:#f9f9f9;border-radius:8px;padding:16px 20px;">
                                <p style="margin:0 0 4px;font-size:11px;color:#999999;text-transform:uppercase;letter-spacing:1px;">Browser</p>
                                <p style="margin:0;font-size:15px;font-weight:500;color:#0f0f0f;">🌐 {Encode(browser)}</p>
                              </td>
                              <td width="4%"></td>
                              <td width="48%" valign="top" style="background:#f9f9f9;border-radius:8px;padding:16px 20px;">
                                <p style="margin:0 0 4px;font-size:11px;color:#999999;text-transform:uppercase;letter-spacing:1px;">OS</p>
                                <p style="margin:0;font-size:15px;font-weight:500;color:#0f0f0f;">💻 {Encode(os)}</p>
                              </td>
                            </tr>
                          </table>

                        </td>
                      </tr>

                      <!-- FOOTER -->
                      <tr>
                        <td style="background:#f9f9f9;padding:20px 40px;text-align:center;border-top:1px solid #eeeeee;">
                          <p style="margin:0;font-size:12px;color:#aaaaaa;">
                            Sent by your Portfolio Tracker · Automated alert
                          </p>
                        </td>
                      </tr>

                    </table>
                  </td>
                </tr>
              </table>

            </body>
            </html>
            """;
        }

        private string Encode(string value) =>
            WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(value) ? "-" : value);
    }
}
