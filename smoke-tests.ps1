# Simple smoke test runner for local development
param()

$urls = @(
	'https://localhost:7123/swagger/index.html',
	'https://localhost:7123/health',
	'https://localhost:7173'
)

function Test-Url($url) {
	try {
		$r = Invoke-WebRequest -Uri $url -UseBasicParsing -Method Head -TimeoutSec 5
		return $r.StatusCode -ge 200 -and $r.StatusCode -lt 400
	} catch {
		return $false
	}
}

foreach ($u in $urls) {
	Write-Host "Checking: $u"
	if (Test-Url $u) { Write-Host "OK" } else { Write-Warning "FAILED" }
}
