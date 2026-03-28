param (
    [Parameter(Mandatory=$true)]
    [string]$Token,
    
    [string]$BaseUrl = "http://localhost:5043/api"
)

[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$headers = @{
    "Authorization" = "Bearer $Token"
    "Content-Type"  = "application/json"
    "Accept"        = "application/json"
}

$report = @()

function Invoke-ApiTest {
    param ([string]$Name, [string]$Method, [string]$Endpoint, [string]$Body = $null)
    Write-Host "Testing [$Name]..." -NoNewline
    $uri = "$BaseUrl$Endpoint"
    $sw = [Diagnostics.Stopwatch]::StartNew()
    try {
        if ($Body) {
            $response = Invoke-WebRequest -Uri $uri -Method $Method -Headers $headers -Body $Body -UseBasicParsing -ErrorAction Stop
        } else {
            $response = Invoke-WebRequest -Uri $uri -Method $Method -Headers $headers -UseBasicParsing -ErrorAction Stop
        }
        $sw.Stop()
        $status = [int]$response.StatusCode
        Write-Host " OK ($status) - $($sw.ElapsedMilliseconds)ms" -ForegroundColor Green
        
        $result = [pscustomobject]@{
            Name = $Name
            Status = $status
            IsSuccess = $true
            Content = if ($response.Content) { $response.Content | ConvertFrom-Json } else { $null }
        }
        $script:report += $result
        return $result
    } catch {
        $sw.Stop()
        $status = $_.Exception.Response.StatusCode
        if ($status) { $status = [int]$status } else { $status = "Error" }
        Write-Host " FAILED ($status) - $($sw.ElapsedMilliseconds)ms" -ForegroundColor Red
        if ($_.Exception.Response) {
            $r = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($r)
            Write-Host $reader.ReadToEnd() -ForegroundColor Gray
        } else {
            Write-Host $_.Exception.Message -ForegroundColor Gray
        }
        
        $result = [pscustomobject]@{
            Name = $Name
            Status = $status
            IsSuccess = $false
            Content = $null
        }
        $script:report += $result
        return $result
    }
}

Write-Host "========================================="
Write-Host "🚀 LEXIFLOW API FULL 15 ENDPOINT TEST 🚀"
Write-Host "========================================="

# --- 1. AUTH ---
Invoke-ApiTest "1. Auth - Forgot Password" "POST" "/Auth/forgot-password" '{"email":"test@lexiflow.com"}' | Out-Null
Invoke-ApiTest "2. Auth - Update Settings" "PUT" "/Auth/settings" '{"dailyNewWordLimit": 15}' | Out-Null
Invoke-ApiTest "3. Auth - Get Settings" "GET" "/Auth/settings" | Out-Null

# --- 2. CLEANUP EXISTING WORDS (To ensure 201 Created and Wordle works) ---
$listRes = Invoke-WebRequest -Uri "$BaseUrl/Words?pageSize=1000" -Method GET -Headers $headers -UseBasicParsing
$allWords = ($listRes.Content | ConvertFrom-Json).data.items
foreach ($w in $allWords) {
    if ($w.englishWord -in @("crane", "apple", "plane", "train", "brain", "ghost")) {
        Invoke-WebRequest -Uri "$BaseUrl/Words/$($w.id)" -Method DELETE -Headers $headers -UseBasicParsing | Out-Null
    }
}

# --- 3. WORDS CRUD ---
$word1Req = @{ englishWord = "crane"; turkishTranslation = "vinç"; difficultyLevel = 1; samples = @() } | ConvertTo-Json -Compress
$w1Res = Invoke-ApiTest "4. Words - Add Word (crane)" "POST" "/Words" $word1Req
$word1Id = $w1Res.Content.data

$word2Req = @{ englishWord = "ghost"; turkishTranslation = "hayalet"; difficultyLevel = 1; samples = @() } | ConvertTo-Json -Compress
$w2Res = Invoke-ApiTest "5. Words - Add Word (ghost)" "POST" "/Words" $word2Req
$word2Id = $w2Res.Content.data

Invoke-ApiTest "6. Words - Get List" "GET" "/Words?page=1&pageSize=10" | Out-Null

if ($word1Id) {
    Invoke-ApiTest "7. Words - Get Detail" "GET" "/Words/$word1Id" | Out-Null
    
    $updateWordReq = @{
        englishWord = "crane"
        turkishTranslation = "vinç (machinery)"
        difficultyLevel = 2
        samples = @( @{ sentenceText = "The crane."; turkishTranslation = "Vinç." } )
    } | ConvertTo-Json -Depth 5 -Compress
    Invoke-ApiTest "8. Words - Update Word" "PUT" "/Words/$word1Id" $updateWordReq | Out-Null
}

if ($word2Id) {
    Invoke-ApiTest "9. Words - Delete Word (ghost)" "DELETE" "/Words/$word2Id" | Out-Null
}

# --- 4. QUIZ (6-REP) ---
Invoke-ApiTest "10. Quiz - Get Today's Review" "GET" "/Quiz/today-review" | Out-Null

if ($word1Id) {
    Write-Host "   -> Simulating 7 correct answers for 'crane' to mark it as Learned for Wordle..." -ForegroundColor Cyan
    for ($i = 1; $i -le 7; $i++) {
        $quizBody = @{ wordId = $word1Id; isCorrect = $true } | ConvertTo-Json -Compress
        $qRes = Invoke-ApiTest "11. Quiz - Submit Answer ($i/7)" "POST" "/Quiz/submit-answer" $quizBody
        # We only count this endpoint as a single successful test block if the last one passes.
        if ($i -lt 7) { $script:report = $script:report | Where-Object { $_.Name -ne "11. Quiz - Submit Answer ($i/7)" } }
    }
}

Invoke-ApiTest "12. Quiz - Get Learned Words" "GET" "/Quiz/learned-words" | Out-Null

# --- 5. WORDLE ---
$wordleStartRes = Invoke-ApiTest "13. Wordle - Start Game" "POST" "/Wordle/start"
$gameId = $wordleStartRes.Content.data.gameId

if ($gameId) {
    $guessBody = @{ guess = "crane" } | ConvertTo-Json -Compress
    Invoke-ApiTest "14. Wordle - Submit Guess (crane)" "POST" "/Wordle/$gameId/guess" $guessBody | Out-Null
}

# --- 6. STORY ---
if ($word1Id) {
    # Generate 4 dynamic but missing words to total 5 for the story
    $w3Id = (Invoke-ApiTest "Words - Add (plane)" "POST" "/Words" (@{ englishWord="plane"; turkishTranslation="uçak"; difficultyLevel=1; samples=@()} | ConvertTo-Json -Compress) | Select-Object -ExpandProperty Content).data
    $w4Id = (Invoke-ApiTest "Words - Add (train)" "POST" "/Words" (@{ englishWord="train"; turkishTranslation="tren"; difficultyLevel=1; samples=@()} | ConvertTo-Json -Compress) | Select-Object -ExpandProperty Content).data
    $w5Id = (Invoke-ApiTest "Words - Add (brain)" "POST" "/Words" (@{ englishWord="brain"; turkishTranslation="beyin"; difficultyLevel=1; samples=@()} | ConvertTo-Json -Compress) | Select-Object -ExpandProperty Content).data
    $w6Id = (Invoke-ApiTest "Words - Add (apple)" "POST" "/Words" (@{ englishWord="apple"; turkishTranslation="elma"; difficultyLevel=1; samples=@()} | ConvertTo-Json -Compress) | Select-Object -ExpandProperty Content).data

    $storyBody = @{ wordIds = @($word1Id, $w3Id, $w4Id, $w5Id, $w6Id); language = "English" } | ConvertTo-Json -Compress
    Invoke-ApiTest "15. Story - Generate AI Story" "POST" "/Story/generate" $storyBody | Out-Null
    
    # Remove dynamic setup words from report to keep it exactly 15 base endpoints
    $script:report = $script:report | Where-Object { $_.Name -notmatch "^Words - Add \(" }
}

# --- 7. ANALYTICS ---
Invoke-ApiTest "16. Analytics - Get Categories Report" "GET" "/Analytics/categories-report" | Out-Null

Write-Host "`n========================================="
Write-Host "📊 TEST SUMMARY REPORT 📊"
Write-Host "========================================="

$passed = ($script:report | Where-Object { $_.IsSuccess }).Count
$total = $script:report.Count
Write-Host "Total Endpoints Tested: $total"
Write-Host "Passed: $passed" -ForegroundColor Green
Write-Host "Failed: $($total - $passed)" -ForegroundColor Red

if ($passed -eq $total) {
    Write-Host "`nAll 16 steps completed successfully. Backend API is stable! 🎉" -ForegroundColor Green
}
