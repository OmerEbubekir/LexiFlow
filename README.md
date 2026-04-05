# 🚀 LearnLanguage — Spaced Repetition Dil Öğrenme Platformu

> **LearnLanguage**, kullanıcıların yabancı dil kelimelerini **Aralıklı Tekrar (Spaced Repetition)** algoritması ile kalıcı hafızaya aktarmasını sağlayan tam yığın (full-stack) bir dil öğrenme platformudur.

---

## 📑 İçindekiler

- [Sistem Mimarisi](#️-sistem-mimarisi)
- [Monorepo Yapısı](#-monorepo-yapısı)
- [Teknoloji Yığını](#️-teknoloji-yığını)
- [Hızlı Başlangıç](#-hızlı-başlangıç)
- [Genel API Sözleşmesi](#-genel-api-sözleşmesi-contract)
- [Global Geliştirme Kuralları](#-global-geliştirme-kuralları)
- [Ortam Değişkenleri Özeti](#-ortam-değişkenleri-özeti)
- [Katkı Sağlama](#-katkı-sağlama)

---

## 🏗️ Sistem Mimarisi

```
┌─────────────────────────────────────────────────────┐
│                   LearnLanguage                      │
│                                                     │
│  ┌──────────┐    ┌──────────┐    ┌───────────────┐ │
│  │  Web UI  │    │  Mobile  │    │    Backend    │ │
│  │  (React/ │    │  (React  │    │   (.NET 8     │ │
│  │   Vite)  │    │  Native) │    │   Web API)    │ │
│  └────┬─────┘    └────┬─────┘    └───────┬───────┘ │
│       │               │                  │         │
│       └───────────────┴──────────────────┘         │
│                        │                            │
│              ┌─────────▼──────────┐                │
│              │  PostgreSQL (UTC)  │                │
│              └────────────────────┘                │
│                                                     │
│  🔐 Firebase Auth  |  🤖 Gemini API                │
└─────────────────────────────────────────────────────┘
```

| Katman | Teknoloji | Açıklama |
|---|---|---|
| **Backend API** | .NET 8, EF Core | İş kuralları, CQRS, Spaced Repetition |
| **Web UI** | React 18, Vite, TypeScript | Tarayıcı tabanlı kullanıcı arayüzü |
| **Mobile** | React Native (Expo), TypeScript | iOS & Android uygulama |
| **Veritabanı** | PostgreSQL | Tüm kalıcı veriler (UTC timezone) |
| **Kimlik Doğrulama** | Firebase Authentication | JIT Provisioning ile PostgreSQL senkronizasyonu |
| **Yapay Zeka** | Google Gemini API | Öğrenilen kelimelerden hikaye üretimi |

---

## 📂 Monorepo Yapısı

```
LearnLanguage/
├── apps/
│   ├── backend/          # .NET 8 Web API (Clean Architecture)
│   │   └── src/
│   │       ├── Core/
│   │       │   ├── Domain/       # Entity'ler, Interface'ler (bağımlılık yok)
│   │       │   └── Application/  # İş kuralları, CQRS, DTO'lar, MediatR
│   │       ├── Infrastructure/   # EF Core, PostgreSQL, Firebase Admin SDK, Polly
│   │       └── WebApi/           # Controller'lar, Middleware, Program.cs
│   │
│   ├── web/              # React + Vite Web Uygulaması
│   │   └── src/
│   │       ├── components/  # Paylaşılan UI bileşenleri
│   │       ├── pages/       # Sayfa bileşenleri
│   │       ├── services/    # Axios API katmanı
│   │       ├── store/       # Zustand state yönetimi
│   │       └── hooks/       # Özel React hook'ları
│   │
│   └── mobil/            # React Native (Expo) Mobil Uygulama
│       └── src/
│           ├── components/  # Tekrar kullanılabilir UI bileşenleri
│           ├── screens/     # Ekranlar (LoginScreen, QuizScreen vb.)
│           ├── navigation/  # React Navigation ayarları
│           ├── services/    # Axios API çağrıları
│           └── store/       # Zustand state yönetimi
│
└── packages/
    └── shared/           # Web ve Mobil arasında paylaşılan TypeScript tipleri
        ├── types/        # Ortak DTO arayüzleri
        └── utils/        # Ortak yardımcı fonksiyonlar
```

---

## 🛠️ Teknoloji Yığını

### Backend
| Paket | Sürüm | Amaç |
|---|---|---|
| .NET | 8.0 | Ana çalışma zamanı |
| EF Core | 8.x | ORM — Code First migrations |
| MediatR | 12.x | CQRS pattern |
| Polly | 8.x | Resilience & retry politikaları |
| Firebase Admin SDK | latest | Token doğrulama & servis hesabı |

### Frontend / Mobile
| Paket | Kullanım Alanı | Amaç |
|---|---|---|
| React 18 + Vite | Web | UI framework |
| React Native (Expo) | Mobile | iOS & Android framework |
| TypeScript | Web & Mobile | Tip güvenliği |
| Zustand | Web & Mobile | Hafif state yönetimi |
| TanStack Query | Web & Mobile | Sunucu durumu ve cache yönetimi |
| Axios | Web & Mobile | HTTP istemcisi (Interceptor ile) |
| Tailwind CSS | Web | Utility-first CSS framework |
| React Navigation | Mobile | Navigasyon (Stack & Tab) |

---

## ⚡ Hızlı Başlangıç

### Ön Koşullar

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) ve npm
- [PostgreSQL 15+](https://www.postgresql.org/) (veya Docker)
- [Expo CLI](https://docs.expo.dev/get-started/installation/) (Mobil için)
- Firebase Projesi (Service Account JSON)

### 1. Repoyu Klonlayın

```bash
git clone <repo-url> LearnLanguage
cd LearnLanguage
```

### 2. Bağımlılıkları Yükleyin

```bash
# Node.js bağımlılıkları (Web + Mobile)
npm install

# .NET bağımlılıkları
cd apps/backend/src/WebApi
dotnet restore
```

### 3. Ortam Değişkenlerini Ayarlayın

Her uygulama için kendi README dosyasına bakınız:
- [`apps/backend/README.md`](./apps/backend/README.md) — `appsettings.Development.json`
- [`apps/web/README.md`](./apps/web/README.md) — `.env`
- [`apps/mobil/README.md`](./apps/mobil/README.md) — `.env`

### 4. Uygulamaları Başlatın

```bash
# Backend
cd apps/backend/src/WebApi
dotnet ef database update --project ../Infrastructure --startup-project .
dotnet run

# Web (ayrı terminalde)
npx vite --workspace=web    # veya projenin kendi dizininde: npm run dev

# Mobile (ayrı terminalde)
npx expo start --workspace=mobile
```

---

## 📡 Genel API Sözleşmesi (Contract)

> Tüm API endpoint'leri için Swagger UI'a `https://localhost:5001/swagger` adresinden erişilebilir.

### ✅ Başarılı İstek Yanıtı

HTTP durum kodu: `200 OK`, `201 Created`

```json
{
  "success": true,
  "data": {
    // Endpoint'e özgü veri nesnesi
  }
}
```

#### Örnek — Kelime Listesi Dönen Yanıt

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
    "pageSize": 20
  }
}
```

### ❌ Hatalı İstek Yanıtı (RFC 7807 Problem Details)

HTTP durum kodu: `4xx`, `5xx`

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Validation Error",
  "status": 422,
  "detail": "Kelime boş olamaz.",
  "instance": "/api/words",
  "errors": {
    "word": ["'Word' alanı boş olamaz."]
  }
}
```

### 🔐 Kimlik Doğrulama

Tüm korumalı endpoint'ler **Bearer Token** gerektirmektedir:

```http
Authorization: Bearer <Firebase_JWT_Token>
```

Token Firebase Authentication'dan alınır. Axios Interceptor bu işlemi otomatik olarak yönetir.

### 📋 Temel Endpoint'ler (Özet)

| Method | Endpoint | Açıklama | Auth |
|---|---|---|---|
| `POST` | `/api/auth/provision` | JIT — Yeni kullanıcıyı PostgreSQL'e ekle | ✅ |
| `GET` | `/api/words` | Kullanıcının kelime listesi (sayfalı) | ✅ |
| `POST` | `/api/words` | Yeni kelime ekle | ✅ |
| `PUT` | `/api/words/{id}` | Kelime güncelle | ✅ |
| `DELETE` | `/api/words/{id}` | Kelime sil | ✅ |
| `GET` | `/api/quiz/today` | Bugün tekrar edilecek kelimeler | ✅ |
| `POST` | `/api/quiz/answer` | Tekrar sonucunu kaydet (SM-2 güncelle) | ✅ |
| `POST` | `/api/story/generate` | Kelimelerden Gemini ile hikaye üret | ✅ |

> ⚠️ Detaylı request/response şemaları için Swagger dokümantasyonuna bakınız.

---

## 📜 Global Geliştirme Kuralları

### Branch İsimlendirme

| Tür | Format | Örnek |
|---|---|---|
| Yeni özellik | `feature/kisa-aciklama` | `feature/quiz-algorithm` |
| Hata düzeltme | `bugfix/kisa-aciklama` | `bugfix/token-timeout` |
| Refactoring | `refactor/kisa-aciklama` | `refactor/word-service` |
| Hotfix | `hotfix/kisa-aciklama` | `hotfix/auth-crash` |

### Commit Mesajları

[Conventional Commits](https://www.conventionalcommits.org/) standardı uygulanır:

```
feat: quiz sayfasına streak sayacı eklendi
fix: token yenileme sırasında oluşan null reference hatası giderildi
refactor: WordService, CQRS pattern'e göre yeniden düzenlendi
docs: backend README güncellendi
chore: bağımlılıklar güncellendi
```

### Timezone (Zaman Dilimi) Kuralı

> ⚠️ **KRİTİK:** Veritabanı ve sunucu her zaman `UTC` çalışır. İstemciler kendi yerel zaman dilimlerini kullanabilir, ancak API'ye gönderilen tüm tarih/saat değerleri ISO 8601 UTC formatında olmalıdır. Örn: `2026-03-06T10:00:00Z`

### Pull Request (PR) Kuralları

1. `main` dalına **doğrudan push yasaktır**.
2. Her PR için en az **1 onay (Approve)** gereklidir.
3. PR açıklamasında yapılan değişiklikler açıkça belirtilmelidir.
4. CI/CD pipeline'ı geçmeyen PR'lar merge edilmez.

---

## 🔑 Ortam Değişkenleri Özeti

| Değişken | Uygulama | Açıklama |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Backend | PostgreSQL bağlantı dizesi |
| `Firebase__ProjectId` | Backend | Firebase proje ID'si |
| `GeminiApi__ApiKey` | Backend | Gemini API anahtarı |
| `VITE_API_BASE_URL` | Web | Backend API base URL |
| `VITE_FIREBASE_API_KEY` | Web | Firebase Web API anahtarı |
| `VITE_FIREBASE_AUTH_DOMAIN` | Web | Firebase Auth domain |
| `EXPO_PUBLIC_API_BASE_URL` | Mobile | Backend API base URL |
| `EXPO_PUBLIC_FIREBASE_API_KEY` | Mobile | Firebase API anahtarı |

---

## 🗺️ Frontend Geliştirme Yol Haritası (Phases)

Backend geliştirimi (Faz 1-8) başarıyla tamamlandığı için proje artık kullanıcı arayüzlerinin inşasına geçmektedir. Aşama aşama ilerlenecek Monorepo Frontend mimari adımları şunlardır:

### 🧩 Phase 9: `packages/shared` Altyapısının Kurulması (✅ TAMAMLANDI)
Frontend mimarisinin omurgası. Web ve Mobil projelerinde kullanılacak ortak yapıların inşası.
- **Tipler:** Backend API sözleşmesine uygun DTO'ların `types/` altında tanımlanması (Örn: `WordDto`, `AddWordCommand`, `GeminiStoryResponse`).
- **API İstemcisi:** Axios instance ve Interceptor ayarları (Token ekleme, JIT provision, yetki hatası yakalama).
- **Servis Katmanı:** Word, Quiz, Dashboard ve AI için ortak API çağrı fonksiyonları.
- **State Yönetimi:** Zustand ile merkezi `useAuthStore` veya `useWordStore` yapıları (eğer framework-agnostic yapılabiliyorsa veya hooks olarak ayrılacaksa).

### 🌐 Phase 10: `apps/web` Kimlik Doğrulama ve İskelet Yapı
React + Vite projesi ayağa kaldırılıp temel iskelet oluşturulacak.
- **Kurulum:** Tailwind CSS konfigürasyonu, React Router entegrası.
- **Auth Modülü:** Firebase login/register sayfaları, Private Route hook'ları.
- **Layout:** Sidebar, Navbar ve genel sayfa Container tasarımları.

### 📚 Phase 11: `apps/web` Kelime Yönetimi ve Dashboard
Temel uygulamanın kalbi.
- **Dashboard:** Günlük öğrenme istatistikleri, Wordle ve hikaye oyunlarına giriş noktaları.
- **Kelime Havuzu:** Sisteme kelime ekleme, silme, listeleme ve düzenleme (Table/Grid view).

### 🧠 Phase 12: `apps/web` Spaced Repetition (6-Rep) ve Quiz
Aralıklı tekrar algoritmasının arayüzü.
- **Quiz Ekranı:** O gün tekrar edilmesi gereken (6-Rep SM-2 tabanlı) kelimelerin kart animasyonuyla ekrana gelmesi.
- **Feedback Sistemi:** Kullanıcının tahmin başarı seviyesini (Ease Factor) gönderen butonlar (Zor, Normal, Kolay).

### 🤖 Phase 13: `apps/web` Gemini AI Hikaye ve Wordle Oyunları
Uygulamayı eğlenceli kılan gamification unsurları.
- **AI Story:** Öğrenilen kelimelerden 5-10 tanesini seçerek Gemini API üzerinden İngilizce hikaye üretme ve okuma arayüzü.
- **Wordle:** Sadece kullanıcının *öğrenmiş olduğu* (IsLearned=true) 5 harfli kelimeler içinden seçilen hedef kelime ile Wordle klonu oynama sayfası.

### 📱 Phase 14: `apps/mobile` Expo Altyapısı ve Auth
React Native tarafının başlangıcı.
- **Kurulum:** Expo Router yapısı, NativeWind konfigürasyonu (Tailwind for React Native).
- **Auth:** Firebase mobil Login/Register ekranları.
- **Paylaşım:** `packages/shared` modülünün sorunsuz şekilde mobile dahil edilmesi.

### 🚀 Phase 15: `apps/mobile` Core Özelliklerin Entegrasyonu
Web'deki özelliklerin mobile taşınması.
- **Görünümler:** Kelime listesi, Quiz kaydırma (Swipe) kartları.
- **Gamification:** Mobil Wordle arayüzü ve AI Story okuma deneyimi.

---

## 🤝 Katkı Sağlama

1. Repoyu **fork**'layın
2. `feature/ozellik-adi` dalı oluşturun
3. Değişikliklerinizi commit'leyin (Conventional Commits)
4. Pull Request açın — en az 1 review bekleyin
5. Merge sonrası dalınızı silin

---

*Son güncelleme: Mart 2026*