namespace Voycar.Api.Web.Service;

public static class Templates
{
    public const string HtmlVerifyTemplate =
        """
              <!DOCTYPE html>
                  <html lang='de'>
                      <head>
                          <meta charset='UTF-8'>
                          <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                          <title>Voycar-Konto-Verifizierung</title>
                      </head>
                      <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                          <div
                              style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                              <h1 style='color: #333333; text-align: center;'>Voycar-Konto-Verifizierung</h1>
                              <p>Hallo {name},</p>
                              <p>vielen Dank, dass du dich bei Voycar registriert hast!</p>
                              <p>Damit wir deine Registrierung abschließen können, musst du deine Email-Adresse bestätigen.</p>
                              <p>Bitte klicke auf den folgenden Link, um dein Konto zu verifizieren:</p>
                              <p style='text-align: center;'>
                                  <a href='{verificationLink}'
                                     style='display: inline-block; background-color: #4CAF50; color: #ffffff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Konto
                                      verifizieren</a>
                              </p>
                              <p>Mit freundlichen Grüßen<br>dein Voycar-Team</p>
                          </div>
                      </body>
                  </html>
            """;

    public const string HtmlResetPasswordTemplate =
        """
            <!DOCTYPE html>
            <html lang='de'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Voycar-Passwort-Reset</title>
                </head>
                <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                    <div
                        style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                        <h1 style='color: #333333; text-align: center;'>Voycar-Passwort-Reset</h1>
                        <p>Bitte klicke auf den folgenden Link, um dein Passwort zurückzusetzen:</p>
                        <p style='text-align: center;'>
                            <a href='{passwordResetLink}'
                               style='display: inline-block; background-color: #4CAF50; color: #ffffff; padding: 10px 20px; text-decoration: none;
                               border-radius: 5px;'>Passwort zurücksetzen</a>
                        </p>
                        <p>Mit freundlichen Grüßen<br>dein Voycar-Team</p>
                    </div>
                </body>
            </html>
        """;
}
