$files = Get-ChildItem -Path "Domain\Entities" -Filter "*.cs"
foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $content = $content -replace "namespace PortfolioApi.Models;", "namespace PortfolioApi.Domain.Entities;"
    Set-Content $file.FullName $content
}
