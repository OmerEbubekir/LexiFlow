# 🌐 LearnLanguage — Web UI (React + Vite)

> Bu paket, LearnLanguage platformunun tarayıcı tabanlı kullanıcı arayüzüdür. React 18 ve Vite ile geliştirilmiştir.

---

## 📑 İçindekiler

- [Teknoloji Yığını](#️-teknoloji-yığını)
- [Tasarım Sistemi](#-tasarım-sistemi-design-system)
- [Klasör Yapısı](#-klasör-yapısı)
- [Geliştirme Ortamı Kurulumu](#-geliştirme-ortamı-kurulumu)
- [Ortam Değişkenleri](#-ortam-değişkenleri-env)
- [API İstekleri & Axios Interceptor](#-api-i̇stekleri--axios-interceptor)
- [State Yönetimi (Zustand)](#-state-yönetimi-zustand)
- [Kimlik Doğrulama Akışı](#-kimlik-doğrulama-akışı-firebase)
- [Mevcut Sayfalar](#-mevcut-sayfalar)

---

## 🛠️ Teknoloji Yığını

| Paket | Sürüm | Amaç |
|---|---|---|
| **React** | 18+ | UI framework |
| **Vite** | 5+ | Build aracı ve geliştirme sunucusu |
| **TypeScript** | 5+ | Tip güvenliği |
| **Tailwind CSS** | 3+ | Utility-first CSS framework |
| **Zustand** | 4+ | Hafif global state yönetimi |
| **TanStack Query** | 5+ | Sunucu durumu, cache ve senkronizasyon |
| **Axios** | 1+ | HTTP istemcisi (Interceptor ile) |
| **Firebase JS SDK** | 10+ | Kimlik doğrulama (Auth) |
| **React Router** | 6+ | İstemci taraflı yönlendirme |

---

## 🎨 Tasarım Sistemi (Design System)

Figma standartlarına göre `tailwind.config.js` dosyasına aşağıdaki renkler tanımlanmıştır:

```js
// tailwind.config.js
module.exports = {
  theme: {
    extend: {
      colors: {
        primary:    '#1CB0F6', // Ana butonlar, vurgular, linkler
        secondary:  '#FFC800', // Streak ateşleri, rozetler, uyarılar
        success:    '#58CC02', // Doğru cevap, tamamlanan kart
        danger:     '#FF4B4B', // Yanlış cevap, silme işlemleri
        background: '#F7F7F7', // Genel uygulama arka planı
        surface:    '#FFFFFF', // Kart ve panel arka planı
        text:       {
          primary:   '#1C1C1C', // Ana metin
          secondary: '#737373', // İkincil / yardımcı metin
        }
      },
      fontFamily: {
        sans: ['DIN Round Pro', 'Nunito', 'ui-sans-serif', 'system-ui'],
      }
    }
  }
}
```

### Stil Kuralları

- Bileşenler Tailwind utility class'ları ile stillendirilir.
- Özel ve tekrarlayan stiller `@layer components` içinde tanımlanır.
- Inline `style` props **kullanılmaz**, tüm stiller Tailwind üzerinden yönetilir.

---

## 📂 Klasör Yapısı

```
apps/web/
└── src/
    ├── components/          # Tekrar kullanılabilir UI bileşenleri
    │   ├── ui/              # Button, Card, Input, Badge vb.
    │   ├── layout/          # Navbar, Sidebar, PageWrapper vb.
    │   └── quiz/            # QuizCard, ProgressBar vb.
    │
    ├── pages/               # Sayfa bileşenleri (route bazlı)
    │   ├── LoginPage.tsx
    │   ├── DashboardPage.tsx
    │   ├── WordsPage.tsx
    │   ├── QuizPage.tsx
    │   └── StoryPage.tsx
    │
    ├── services/            # API çağrı fonksiyonları
    │   ├── axiosInstance.ts # Yapılandırılmış Axios instance + Interceptor
    │   ├── wordService.ts   # Kelime CRUD fonksiyonları
    │   ├── quizService.ts   # Quiz API çağrıları
    │   └── storyService.ts  # Hikaye üretme API çağrısı
    │
    ├── store/               # Zustand store'ları
    │   ├── authStore.ts     # Kullanıcı oturum bilgileri
    │   └── quizStore.ts     # Aktif quiz oturum durumu
    │
    ├── hooks/               # Özel React hook'ları
    │   ├── useAuth.ts       # Firebase auth durumu
    │   └── useWords.ts      # TanStack Query ile kelime listesi
    │
    ├── types/               # Proje geneli TypeScript tipleri
    │   └── index.ts         # Word, QuizItem, User vb. (packages/shared'dan re-export)
    │
    ├── router/              # React Router tanımlamaları
    │   └── index.tsx        # Route yapısı ve korumalı route'lar
    │
    ├── App.tsx
    ├── main.tsx
    └── index.css            # Global stiller + Tailwind direktifleri
```

---

## 🚀 Geliştirme Ortamı Kurulumu

### 1. Bağımlılıkları Yükleyin

Monorepo kök dizininde:

```bash
npm install
```

### 2. Ortam Değişkenlerini Ayarlayın

`apps/web/` dizininde `.env` dosyası oluşturun (aşağıya bakınız).

### 3. Geliştirme Sunucusunu Başlatın

```bash
# Monorepo kökünden
npx vite --workspace=web

# Veya apps/web/ dizinindeyken
npm run dev
```

Uygulama `http://localhost:5173` adresinde açılacaktır.

---

## 🔑 Ortam Değişkenleri (`.env`)

`apps/web/` dizinine `.env` dosyası oluşturun. `.env` dosyası **asla Git'e eklenmemelidir**.

```env
# Backend API'nin temel URL'i
VITE_API_BASE_URL=https://localhost:5001/api

# Firebase Konfigürasyonu
# Firebase konsolundan: Proje Ayarları > Genel > Web Uygulamanız
VITE_FIREBASE_API_KEY=your-firebase-api-key
VITE_FIREBASE_AUTH_DOMAIN=your-project-id.firebaseapp.com
VITE_FIREBASE_PROJECT_ID=your-project-id
VITE_FIREBASE_STORAGE_BUCKET=your-project-id.appspot.com
VITE_FIREBASE_MESSAGING_SENDER_ID=123456789012
VITE_FIREBASE_APP_ID=1:123456789012:web:abcdef1234567890
```

> ⚠️ Tüm değişkenler `VITE_` prefix'iyle başlamalıdır, aksi hâlde Vite tarafından erişilemez.

---

## 📡 API İstekleri & Axios Interceptor

Tüm API istekleri `src/services/axiosInstance.ts` üzerinden yapılmalıdır. Bu dosyada tanımlı **Axios Interceptor**, her istekten önce Firebase'den güncel JWT token'ı alarak `Authorization: Bearer <token>` header'ına otomatik ekler.

```typescript
// src/services/axiosInstance.ts
import axios from 'axios';
import { auth } from '../firebase';

const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
});

// Request Interceptor — Her isteğe token ekle
axiosInstance.interceptors.request.use(async (config) => {
  const user = auth.currentUser;
  if (user) {
    const token = await user.getIdToken();
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response Interceptor — 401 durumunda oturumu kapat
axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      await auth.signOut();
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default axiosInstance;
```

> 🚫 **Yasaklı:** Component içlerinde `firebase.auth().currentUser.getIdToken()` ile manuel token yönetimi yapmayın. Her zaman `axiosInstance` kullanın.

---

## 🗂️ State Yönetimi (Zustand)

Uygulama geneli durumu yönetmek için **Zustand** kullanılmaktadır.

### `authStore.ts` — Kullanıcı Oturumu

```typescript
interface AuthState {
  user: FirebaseUser | null;
  isLoading: boolean;
  setUser: (user: FirebaseUser | null) => void;
}
```

### `quizStore.ts` — Quiz Oturum Durumu

```typescript
interface QuizState {
  queue: QuizItem[];        // Tekrar edilecek kelime kuyruğu
  currentIndex: number;
  sessionResults: Result[]; // Oturum içi cevap geçmişi
  setQueue: (items: QuizItem[]) => void;
  submitAnswer: (quality: number) => void;
}
```

---

## 🔒 Kimlik Doğrulama Akışı (Firebase)

```
Kullanıcı → Login Sayfası
    │
    ▼
Firebase Email/Password veya Google Sign-In
    │
    ▼
Firebase JWT Token alınır
    │
    ▼
Backend: POST /api/auth/provision
(JIT Provisioning — kullanıcı yoksa PostgreSQL'e eklenir)
    │
    ▼
Auth store güncellenir → Kullanıcı Dashboard'a yönlendirilir
```

**Korumalı Route'lar:**
`/dashboard`, `/words`, `/quiz`, `/story` sayfaları oturum gerektirmektedir. Oturumu olmayan kullanıcılar otomatik olarak `/login` sayfasına yönlendirilir.

---

## 📱 Mevcut Sayfalar

| Sayfa | Route | Açıklama | Auth |
|---|---|---|---|
| Login | `/login` | E-posta/şifre ve Google ile giriş | ❌ |
| Dashboard | `/` | Streak, günlük hedef, istatistik özeti | ✅ |
| Kelimeler | `/words` | Kelime listesi, ekleme ve düzenleme | ✅ |
| Quiz | `/quiz` | Aralıklı tekrar quiz ekranı | ✅ |
| Hikaye | `/story` | Gemini ile hikaye üretme | ✅ |

---

*Son güncelleme: Mart 2026*