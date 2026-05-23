# Run all runnable projects for local integration testing
# Usage: Open PowerShell at the repository root and run: .\run-all.ps1
# Optionally pass -Rebuild to force a rebuild before run: .\run-all.ps1 -Rebuild

param(
	[switch]$Rebuild
)

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Write-Host "Repo root: $root"

if ($Rebuild) {
	Write-Host "Building solution..."
	dotnet build "$root" -c Debug || { Write-Error "Build failed"; exit 1 }
} else {
	Write-Host "Building solution (incremental)..."
	dotnet build "$root" -c Debug || { Write-Error "Build failed"; exit 1 }
}

# Projects that produce runnable apps in this repo
$projectsToRun = @(
	@{ Name = 'OperationalWorkspaceAPI'; CsProj = Join-Path $root 'OperationalWorkspaceAPI\OperationalWorkspaceAPI.csproj' },
	@{ Name = 'OperationalWorkspaceUI'; CsProj = Join-Path $root 'OperationalWorkspaceUI\OperationalWorkspaceUI.csproj' },
	@{ Name = 'OperationalWorkspace.Addin'; CsProj = Join-Path $root 'OperationalWorkspace.Addin\OperationalWorkspace.Addin.csproj' }
)

foreach ($p in $projectsToRun) {
	if (-not (Test-Path $p.CsProj)) {
		Write-Warning "Project file not found: $($p.CsProj) — skipping"
		continue
	}

	Write-Host "Starting $($p.Name)..."

	# Start each project in its own process window so you can inspect logs separately.
	Start-Process -FilePath 'dotnet' -ArgumentList "run --project `"$($p.CsProj)`"" -WorkingDirectory (Split-Path $p.CsProj) -NoNewWindow:$false
}

Write-Host "All runnable projects launched. Waiting for endpoints to become healthy..."

# Simple health checks
function Wait-ForUrl([string]$url, [int]$timeoutSeconds = 30) {
	$deadline = (Get-Date).AddSeconds($timeoutSeconds)
	while ((Get-Date) -lt $deadline) {
		try {
			$r = Invoke-WebRequest -Uri $url -UseBasicParsing -Method Head -TimeoutSec 5
			if ($r.StatusCode -ge 200 -and $r.StatusCode -lt 400) { return $true }
		} catch {
			Start-Sleep -Milliseconds 500
		}
	}
	return $false
}

# API Swagger
$apiUrl = 'https://localhost:7123/swagger/index.html'
if (Wait-ForUrl $apiUrl 45) { Write-Host "API is healthy at $apiUrl" } else { Write-Warning "API did not respond at $apiUrl" }

# UI Root (adjust if your UI listens on a different port)
$uiUrl = 'https://localhost:7173'
if (Wait-ForUrl $uiUrl 45) { Write-Host "UI is healthy at $uiUrl" } else { Write-Warning "UI did not respond at $uiUrl" }

Write-Host "Run complete. Use Task Manager or the opened consoles to stop the running apps."

# Optional: run smoke tests against launched endpoints
function Run-SmokeTests {
	Write-Host "Running smoke tests..."

	$tests = @(
		@{ Name = 'API Swagger'; Url = 'https://localhost:7123/swagger/index.html' },
		@{ Name = 'UI Root'; Url = 'https://localhost:7173' },
		@{ Name = 'API Health (sample)'; Url = 'https://localhost:7123/health' }
	)

	foreach ($t in $tests) {
		Write-Host "Testing $($t.Name) -> $($t.Url)"
		if (Wait-ForUrl $t.Url 20) { Write-Host "OK: $($t.Name)" } else { Write-Warning "FAIL: $($t.Name)" }
	}
}

Run-SmokeTests
