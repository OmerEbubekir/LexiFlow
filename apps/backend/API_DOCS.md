# LexiFlow Backend API Documentation

This document describes all the RESTful endpoints available in the LexiFlow Backend API. 
All endpoints (except `forgot-password`) require a valid Bearer Token from Firebase Auth in the `Authorization` header.

## Base URL
`http://localhost:5043/api` (Local Development)

---

## 1. Auth & User Settings

### 1.1. Forgot Password
**Method:** `POST`
**Route:** `/Auth/forgot-password`
**Description:** Sends a password reset email via Firebase. (No auth required)

**Request Body:**
```json
{
  "email": "user@example.com"
}
```
**Responses:**
- `200 OK`: `{ "success": true, "message": "Email sent." }`
- `400 Bad Request`: `{ "success": false, "errors": ["Email not found"] }`

### 1.2. Get User Settings
**Method:** `GET`
**Route:** `/Auth/settings`
**Description:** Retrieves user-specific application settings.

**Responses:**
- `200 OK`: 
```json
{
  "success": true,
  "data": {
    "dailyNewWordLimit": 15
  }
}
```
- `401 Unauthorized`

### 1.3. Update User Settings
**Method:** `PUT`
**Route:** `/Auth/settings`
**Description:** Updates user-specific application settings.

**Request Body:**
```json
{
  "dailyNewWordLimit": 20
}
```
**Responses:**
- `200 OK`: `{ "success": true }`
- `400 Bad Request`

---

## 2. Words (Vocabulary Management)

### 2.1. Get All Words (Paginated)
**Method:** `GET`
**Route:** `/Words?page=1&pageSize=10`
**Description:** Retrieves a paginated list of words belonging to the current user.

**Responses:**
- `200 OK`: 
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "guid",
        "englishWord": "crane",
        "turkishTranslation": "vinç",
        "difficultyLevel": 1,
        "samples": []
      }
    ],
    "totalCount": 1
  }
}
```

### 2.2. Get Word Detail
**Method:** `GET`
**Route:** `/Words/{id}`
**Description:** Retrieves detailed information of a specific word.

**Responses:**
- `200 OK`: Returning `WordDto`
- `404 Not Found`

### 2.3. Add Word
**Method:** `POST`
**Route:** `/Words`
**Description:** Adds a new word to the user's vocabulary.

**Request Body:**
```json
{
  "englishWord": "crane",
  "turkishTranslation": "vinç",
  "difficultyLevel": 1,
  "categoryId": null,
  "pictureUrl": null,
  "audioUrl": null,
  "samples": [
    {
      "sentenceText": "The crane lifted the container.",
      "turkishTranslation": "Vinç konteyneri kaldırdı."
    }
  ]
}
```
**Responses:**
- `201 Created`: `{ "success": true, "data": "new-word-id" }`
- `400 Bad Request`

### 2.4. Update Word
**Method:** `PUT`
**Route:** `/Words/{id}`
**Description:** Updates an existing word and its samples.

**Request Body:**
```json
{
  "englishWord": "crane",
  "turkishTranslation": "vinç (machinery)",
  "difficultyLevel": 2,
  "categoryId": null,
  "pictureUrl": null,
  "audioUrl": null,
  "samples": [
    {
      "id": "existing-sample-id",
      "sentenceText": "The crane is very tall.",
      "turkishTranslation": "Vinç çok uzun."
    }
  ]
}
```
**Responses:**
- `200 OK`: `{ "success": true }`
- `404 Not Found`

### 2.5. Delete Word
**Method:** `DELETE`
**Route:** `/Words/{id}`
**Description:** Deletes a word from the user's vocabulary.

**Responses:**
- `200 OK`: `{ "success": true }`
- `404 Not Found`

---

## 3. Quiz (Spaced Repetition)

### 3.1. Get Today's Review
**Method:** `GET`
**Route:** `/Quiz/today-review`
**Description:** Retrieves words scheduled for review today based on the 6-Rep Spaced Repetition algorithm.

**Responses:**
- `200 OK`: 
```json
{
  "success": true,
  "data": [
    {
      "userWordProgressId": "guid",
      "word": { ...WordDto... },
      "consecutiveCorrect": 0,
      "nextReviewDate": "2026-04-05T00:00:00Z"
    }
  ]
}
```

### 3.2. Get Learned Words
**Method:** `GET`
**Route:** `/Quiz/learned-words`
**Description:** Retrieves words that have successfully passed all 6 steps of repetition (consecutiveCorrect >= 6).

**Responses:**
- `200 OK`: `{ "success": true, "data": [{...}] }`

### 3.3. Submit Answer
**Method:** `POST`
**Route:** `/Quiz/submit-answer`
**Description:** Submits the result of a single word review (Flashcard format, NOT multiple choice).

**Request Body:**
```json
{
  "wordId": "word-guid",
  "isCorrect": true
}
```
**Responses:**
- `200 OK`: `{ "success": true }`
- `400 Bad Request`

---

## 4. Wordle Game

### 4.1. Start Wordle Game
**Method:** `POST`
**Route:** `/Wordle/start`
**Description:** Starts a new Wordle game session using the user's "Learned" words.

**Responses:**
- `200 OK`:
```json
{
  "success": true,
  "data": {
    "gameId": "guid",
    "targetWordLength": 5,
    "maxAttempts": 6
  }
}
```

### 4.2. Submit Guess
**Method:** `POST`
**Route:** `/Wordle/{gameId}/guess`
**Description:** Submits a guessed word for the active session.

**Request Body:**
```json
{
  "guess": "crane"
}
```
**Responses:**
- `200 OK`:
```json
{
  "success": true,
  "data": {
    "isWon": true,
    "isGameOver": true,
    "patterns": ["🟩", "🟩", "🟩", "🟩", "🟩"]
  }
}
```
- `400 Bad Request`: (e.g., "Game is already over" or "Invalid guess length")

---

## 5. Story Generator (AI)

### 5.1. Generate AI Story
**Method:** `POST`
**Route:** `/Story/generate`
**Description:** Generates a short story utilizing the provided target vocabulary.

**Request Body:**
```json
{
  "wordIds": ["id1", "id2", "id3", "id4", "id5"],
  "language": "English"
}
```
**Responses:**
- `200 OK`:
```json
{
  "success": true,
  "data": {
    "storyText": "Once upon a time, a ghost drove a train...",
    "generatedAt": "2026-04-05T12:00:00Z"
  }
}
```
- `500 Internal Server Error` (If Gemini AI fails)

---

## 6. Analytics

### 6.1. Get Categories Report
**Method:** `GET`
**Route:** `/Analytics/categories-report`
**Description:** Fetches statistics and progress grouped by vocabulary categories.

**Responses:**
- `200 OK`:
```json
{
  "success": true,
  "data": [
    {
      "categoryId": "guid",
      "categoryName": "Animals",
      "totalWords": 50,
      "learnedWords": 10
    }
  ]
}
```
