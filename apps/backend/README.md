# ⚙️ LearnLanguage — Backend (.NET 8 Web API)

> Bu paket, LearnLanguage sisteminin çekirdek API'sini barındırır. Tüm iş kuralları (Spaced Repetition / SM-2 algoritması), veri güvenliği ve dış servis entegrasyonları (Gemini, Firebase) bu katmanda yönetilir.

---

## 📑 İçindekiler

- [Mimari Yapı](#️-mimari-yapı-clean-architecture)
- [Proje Klasör Yapısı](#-proje-klasör-yapısı)
- [Geliştirme Ortamı Kurulumu](#-geliştirme-ortamı-kurulumu)
- [Konfigürasyon](#1-konfigürasyon-appsettingsdevelopmentjson)
- [Veritabanı Migrasyonları](#2-veritabanı-migrasyonları-ef-core)
- [Projeyi Çalıştırma](#3-projeyi-çalıştırma)
- [API Endpoint Referansı](#-api-endpoint-referansı)
- [JSON Sözleşmeleri (Request & Response)](#-json-sözleşmeleri-request--response)
- [Middleware Katmanları](#️-middleware-katmanları)
- [Hata Yönetimi](#-hata-yönetimi)

---

## 🏗️ Mimari Yapı (Clean Architecture)

Proje, bağımlılıkların **dışarıdan içeriye** doğru aktığı 4 katmanlı Temiz Mimari (Clean Architecture) prensibine göre inşa edilmiştir:

```
┌─────────────────────────────────────────┐
│              WebApi Layer               │  ← HTTP, Middleware, Controller
├─────────────────────────────────────────┤
│          Infrastructure Layer           │  ← EF Core, Firebase, Gemini, Polly
├─────────────────────────────────────────┤
│           Application Layer             │  ← CQRS, MediatR, DTO, Validator
├─────────────────────────────────────────┤
│             Domain Layer                │  ← Entity'ler, Interface'ler (sıfır bağımlılık)
└─────────────────────────────────────────┘
         ↑ Bağımlılık yönü: içe doğru
```

| Katman | Yol | Sorumluluk |
|---|---|---|
| **Domain** | `src/Core/Domain` | Entity'ler, Enumlar, Domain Interface'leri. Hiçbir dış kütüphane bağımlılığı yoktur. |
| **Application** | `src/Core/Application` | İş kuralları, CQRS komutları/sorguları, MediatR Handler'ları, DTO'lar, FluentValidation kuralları. |
| **Infrastructure** | `src/Infrastructure` | EF Core `DbContext`, PostgreSQL bağlantısı, Firebase Admin SDK, Gemini API istemcisi, Polly Resilience politikaları. |
| **WebApi** | `src/WebApi` | Controller'lar, Middleware'ler (Global Exception Handler, JIT Provisioning, Rate Limiting), `Program.cs`. |

---

## 📂 Proje Klasör Yapısı

```
apps/backend/
└── src/
    ├── Core/
    │   ├── Domain/
    │   │   ├── Entities/         # Word.cs, User.cs, ReviewSession.cs
    │   │   ├── Interfaces/       # IWordRepository.cs, IUnitOfWork.cs
    │   │   └── Enums/            # ReviewQuality.cs
    │   │
    │   └── Application/
    │       ├── Features/
    │       │   ├── Words/
    │       │   │   ├── Commands/ # AddWordCommand.cs, DeleteWordCommand.cs
    │       │   │   └── Queries/  # GetWordListQuery.cs
    │       │   ├── Quiz/
    │       │   │   ├── Commands/ # SubmitAnswerCommand.cs
    │       │   │   └── Queries/  # GetTodayReviewQuery.cs
    │       │   └── Story/
    │       │       └── Commands/ # GenerateStoryCommand.cs
    │       ├── DTOs/             # WordDto.cs, QuizItemDto.cs, StoryDto.cs
    │       └── Common/           # BaseResponse.cs, PaginatedList.cs
    │
    ├── Infrastructure/
    │   ├── Persistence/
    │   │   ├── AppDbContext.cs
    │   │   └── Migrations/
    │   ├── Repositories/         # WordRepository.cs
    │   ├── ExternalServices/
    │   │   ├── FirebaseService.cs
    │   │   └── GeminiService.cs
    │   └── Resilience/           # Polly politikaları
    │
    └── WebApi/
        ├── Controllers/
        │   ├── WordsController.cs
        │   ├── QuizController.cs
        │   ├── StoryController.cs
        │   └── AuthController.cs
        ├── Middleware/
        │   ├── GlobalExceptionHandlerMiddleware.cs
        │   ├── JitProvisioningMiddleware.cs
        │   └── RateLimitingMiddleware.cs
        ├── appsettings.json
        ├── appsettings.Development.json  ← GİT'E EKLENMEMELİ
        └── Program.cs
```

---

## 🚀 Geliştirme Ortamı Kurulumu

### Ön Koşullar (Prerequisites)

- [**.NET 8.0 SDK**](https://dotnet.microsoft.com/download/dotnet/8.0)
- [**PostgreSQL 15+**](https://www.postgresql.org/download/) — Lokalde Docker ile çalıştırılması tavsiye edilir:
  ```bash
  docker run --name learnlanguage-db \
    -e POSTGRES_PASSWORD=yourpassword \
    -e POSTGRES_DB=LearnLanguageDb \
    -p 5432:5432 -d postgres:15
  ```
- [**Firebase Projesi**](https://console.firebase.google.com/) — Admin SDK Service Account JSON dosyası
- [**EF Core CLI Araçları**](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

### 1. Konfigürasyon (`appsettings.Development.json`)

`src/WebApi/` klasörü içinde `appsettings.Development.json` dosyası oluşturun. Bu dosya `.gitignore`'a eklenmiş olmalıdır — **asla commit'lemeyin**.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LearnLanguageDb;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Firebase": {
    "ProjectId": "your-firebase-project-id",
    "ServiceAccountKeyPath": "secrets/firebase-adminsdk.json"
  },
  "GeminiApi": {
    "ApiKey": "your-gemini-api-key",
    "BaseUrl": "https://generativelanguage.googleapis.com",
    "Model": "gemini-1.5-flash"
  },
  "RateLimit": {
    "PermitLimit": 100,
    "WindowSeconds": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

> 💡 **Firebase Service Account:** Firebase konsolundan indirilen `firebase-adminsdk.json` dosyasını `src/WebApi/secrets/` klasörüne yerleştirin. Bu klasörün de `.gitignore`'da olduğundan emin olun.

---

### 2. Veritabanı Migrasyonları (EF Core)

Code-First yaklaşımı kullanıyoruz. Veritabanını oluşturmak veya mevcut migrasyonları uygulamak için `src/WebApi/` dizinindeyken şu komutu çalıştırın:

```bash
# Veritabanı migrasyonlarını uygula
dotnet ef database update --project ../Infrastructure --startup-project .

# Yeni migration oluştur (şema değişikliği yapıldığında)
dotnet ef migrations add MigrationAdi --project ../Infrastructure --startup-project .

# Son migration'ı geri al
dotnet ef migrations remove --project ../Infrastructure --startup-project .
```

---

### 3. Projeyi Çalıştırma

```bash
cd src/WebApi
dotnet run
```

Uygulama başarıyla ayağa kalktığında:
- **API:** `https://localhost:5001`
- **Swagger UI:** `https://localhost:5001/swagger`
- **HTTP (yönlendirme):** `http://localhost:5000`

---

## 📡 API Endpoint Referansı

> Tüm endpoint'ler `/api` prefix'i ile başlar. Korumalı endpoint'ler `Authorization: Bearer <token>` header'ı gerektirir.

### 🔐 Auth

| Method | Endpoint | Açıklama | Auth |
|---|---|---|---|
| `POST` | `/api/auth/provision` | Firebase token ile JIT kullanıcı provisionlama | ✅ |

### 📚 Words (Kelimeler)

| Method | Endpoint | Açıklama | Auth |
|---|---|---|---|
| `GET` | `/api/words` | Kullanıcının kelime listesi (sayfalandırılmış) | ✅ |
| `POST` | `/api/words` | Yeni kelime ekle | ✅ |
| `PUT` | `/api/words/{id}` | Mevcut kelimeyi güncelle | ✅ |
| `DELETE` | `/api/words/{id}` | Kelime sil | ✅ |

### 🧠 Quiz (Aralıklı Tekrar)

| Method | Endpoint | Açıklama | Auth |
|---|---|---|---|
| `GET` | `/api/quiz/today` | Bugün tekrar edilecek kelimeler | ✅ |
| `POST` | `/api/quiz/answer` | Tekrar sonucunu kaydet, SM-2 algoritması ile interval/ease güncelle | ✅ |

### 📖 Story (Hikaye)

| Method | Endpoint | Açıklama | Auth |
|---|---|---|---|
| `POST` | `/api/story/generate` | Seçilen kelimelerden Gemini API ile hikaye üret | ✅ |

---

## 📋 JSON Sözleşmeleri (Request & Response)

### Genel Başarılı Yanıt Formatı

```json
{
  "success": true,
  "data": { }
}
```

### Genel Hata Yanıtı Formatı (RFC 7807)

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Hata Başlığı",
  "status": 422,
  "detail": "İnsan okunabilir hata açıklaması.",
  "instance": "/api/endpoint",
  "errors": {
    "fieldName": ["Hata mesajı"]
  }
}
```

---

### `POST /api/words` — Kelime Ekle

**Request Body:**
```json
{
  "word": "serendipity",
  "translation": "tesadüfen güzel şeyler bulma yeteneği",
  "exampleSentence": "Finding that old book was pure serendipity.",
  "language": "en"
}
```

**Response `201 Created`:**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "word": "serendipity",
    "translation": "tesadüfen güzel şeyler bulma yeteneği",
    "exampleSentence": "Finding that old book was pure serendipity.",
    "language": "en",
    "easeFactor": 2.5,
    "interval": 1,
    "repetitions": 0,
    "nextReviewDate": "2026-03-07T00:00:00Z",
    "createdAt": "2026-03-06T10:00:00Z"
  }
}
```

---

### `GET /api/words` — Kelime Listesi

**Query Parameters:**

| Parametre | Tip | Varsayılan | Açıklama |
|---|---|---|---|
| `page` | `int` | `1` | Sayfa numarası |
| `pageSize` | `int` | `20` | Sayfa başına kayıt sayısı |
| `language` | `string` | `null` | Dil filtresi (örn: `en`, `de`) |
| `search` | `string` | `null` | Kelime içinde arama |

**Response `200 OK`:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "word": "serendipity",
        "translation": "tesadüfen güzel şeyler bulma yeteneği",
        "easeFactor": 2.5,
        "interval": 4,
        "nextReviewDate": "2026-03-10T00:00:00Z"
      }
    ],
    "totalCount": 42,
    "page": 1,
    "pageSize": 20,
    "totalPages": 3
  }
}
```

---

### `GET /api/quiz/today` — Bugünkü Tekrar Listesi

**Response `200 OK`:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "wordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "word": "serendipity",
        "translation": "tesadüfen güzel şeyler bulma yeteneği",
        "exampleSentence": "Finding that old book was pure serendipity."
      }
    ],
    "count": 8
  }
}
```

---

### `POST /api/quiz/answer` — Tekrar Cevabını Kaydet

SM-2 algoritmasına göre `quality` değeri 0–5 arasında bir tam sayıdır:
- `0–2`: Yanlış / hatırlanamadı → interval sıfırlanır
- `3`: Zor hatırlandı
- `4`: Doğru
- `5`: Çok kolay

**Request Body:**
```json
{
  "wordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "quality": 4
}
```

**Response `200 OK`:**
```json
{
  "success": true,
  "data": {
    "wordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "newInterval": 6,
    "newEaseFactor": 2.6,
    "nextReviewDate": "2026-03-12T00:00:00Z"
  }
}
```

---

### `POST /api/story/generate` — Hikaye Üret

**Request Body:**
```json
{
  "wordIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d"
  ],
  "language": "en",
  "storyLength": "short"
}
```

> `storyLength` kabul edilen değerler: `"short"`, `"medium"`, `"long"`

**Response `200 OK`:**
```json
{
  "success": true,
  "data": {
    "story": "It was pure serendipity when Emma discovered the ephemeral beauty of the old library...",
    "wordsUsed": ["serendipity", "ephemeral"],
    "generatedAt": "2026-03-06T10:05:00Z"
  }
}
```

---

## 🛡️ Middleware Katmanları

| Middleware | Açıklama |
|---|---|
| `GlobalExceptionHandlerMiddleware` | Yakalanmamış tüm exception'ları RFC 7807 formatında döner. |
| `JitProvisioningMiddleware` | Firebase token doğrulandıktan sonra kullanıcı PostgreSQL'de yoksa otomatik oluşturur. |
| `RateLimitingMiddleware` | .NET 8 built-in Rate Limiter — varsayılan 100 istek/dakika (yapılandırılabilir). |

---

## 🔥 Hata Yönetimi

| HTTP Kodu | Durum | Açıklama |
|---|---|---|
| `200` | OK | Başarılı GET/PUT işlemi |
| `201` | Created | Başarılı POST (yeni kaynak oluşturuldu) |
| `400` | Bad Request | Geçersiz istek gövdesi |
| `401` | Unauthorized | Token eksik veya geçersiz |
| `403` | Forbidden | Kaynak başka kullanıcıya ait |
| `404` | Not Found | Kaynak bulunamadı |
| `422` | Unprocessable Entity | Doğrulama hatası (FluentValidation) |
| `429` | Too Many Requests | Rate limit aşıldı |
| `500` | Internal Server Error | Sunucu tarafı beklenmedik hata |

---

*Son güncelleme: Mart 2026*