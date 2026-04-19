# Portfolio API - Environment Variables Setup Script
# Run this script to set all required environment variables for local development
# Note: These are the OLD/DEV values. Use different values for production!

Write-Host "Setting Portfolio API Environment Variables..." -ForegroundColor Green
Write-Host "WARNING: These are development values. Do NOT use in production!" -ForegroundColor Yellow
Write-Host ""

# JWT Configuration
[Environment]::SetEnvironmentVariable("Jwt__Secret", "menomo-portfolio-api-strong-secret-key-32", "User")
Write-Host "✓ Jwt__Secret set" -ForegroundColor Green

# Admin User Configuration
[Environment]::SetEnvironmentVariable("Admin__Username", "Menomo", "User")
Write-Host "✓ Admin__Username set" -ForegroundColor Green

[Environment]::SetEnvironmentVariable("Admin__Password", "Menomo@1234", "User")
Write-Host "✓ Admin__Password set" -ForegroundColor Green

# Email Settings
[Environment]::SetEnvironmentVariable("EmailSettings__SmtpUser", "hnbg14006@gmail.com", "User")
Write-Host "✓ EmailSettings__SmtpUser set" -ForegroundColor Green

[Environment]::SetEnvironmentVariable("EmailSettings__SmtpPass", "vlkk jmat ouli zbgs", "User")
Write-Host "✓ EmailSettings__SmtpPass set" -ForegroundColor Green

# CORS Origins (optional - defaults will work for localhost)
[Environment]::SetEnvironmentVariable("Cors__AllowedOrigins__0", "http://localhost:4200", "User")
Write-Host "✓ Cors__AllowedOrigins__0 set" -ForegroundColor Green

Write-Host ""
Write-Host "All environment variables have been set for the CURRENT USER!" -ForegroundColor Green
Write-Host "You may need to restart your terminal/IDE for changes to take effect." -ForegroundColor Yellow
Write-Host ""
Write-Host "To verify, run: `$env:Jwt__Secret" -ForegroundColor Cyan
