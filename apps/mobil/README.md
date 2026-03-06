# 📱 LearnLanguage — Mobile (React Native + Expo)

> LearnLanguage platformunun iOS ve Android uygulamasıdır. React Native (Expo Managed Workflow) ile geliştirilmiştir.

---

## 📑 İçindekiler

- [Teknoloji Yığını](#️-teknoloji-yığını)
- [Klasör Yapısı](#-klasör-yapısı)
- [Geliştirme Ortamı Kurulumu](#-geliştirme-ortamı-kurulumu)
- [Ortam Değişkenleri](#-ortam-değişkenleri-env)
- [Navigasyon Yapısı](#️-navigasyon-yapısı)
- [API İstekleri & Axios Interceptor](#-api-i̇stekleri--axios-interceptor)
- [State Yönetimi (Zustand)](#-state-yönetimi-zustand)
- [Kimlik Doğrulama Akışı](#-kimlik-doğrulama-akışı-firebase)
- [Mevcut Ekranlar](#-mevcut-ekranlar)
- [Derleme & Yayınlama (EAS Build)](#-derleme--yayınlama-eas-build)

---

## 🛠️ Teknoloji Yığını

| Paket | Sürüm | Amaç |
|---|---|---|
| **React Native** | 0.73+ | iOS & Android UI framework |
| **Expo** | 50+ | Managed Workflow, OTA güncellemeler |
| **TypeScript** | 5+ | Tip güvenliği |
| **React Navigation** | 6+ | Navigasyon (Stack & Tab) |
| **Zustand** | 4+ | Hafif global state yönetimi |
| **TanStack Query** | 5+ | Sunucu durumu, cache ve senkronizasyon |
| **Axios** | 1+ | HTTP istemcisi (Interceptor ile) |
| **Firebase Auth** | 10+ | React Native uyumlu kimlik doğrulama |
| **Expo SecureStore** | latest | Token ve hassas veri güvenli depolama |

---

## 📂 Klasör Yapısı

```
apps/mobil/
└── src/
    ├── components/          # Tekrar kullanılabilir UI bileşenleri
    │   ├── ui/              # Button, Card, Input, Badge, ProgressBar vb.
    │   └── quiz/            # FlipCard, AnswerButton vb.
    │
    ├── screens/             # Ekranlar (her bir navigasyon noktası)
    │   ├── auth/
    │   │   └── LoginScreen.tsx
    │   ├── dashboard/
    │   │   └── DashboardScreen.tsx
    │   ├── words/
    │   │   ├── WordListScreen.tsx
    │   │   └── AddWordScreen.tsx
    │   ├── quiz/
    │   │   └── QuizScreen.tsx
    │   └── story/
    │       └── StoryScreen.tsx
    │
    ├── navigation/          # React Navigation konfigürasyonu
    │   ├── index.tsx        # Ana navigator — auth durumuna göre yönlendir
    │   ├── AppNavigator.tsx # Oturum açık kullanıcılar için navigasyon
    │   └── AuthNavigator.tsx # Oturum açmamış kullanıcılar için navigasyon
    │
    ├── services/            # API katmanı
    │   ├── axiosInstance.ts # Yapılandırılmış Axios instance + Interceptor
    │   ├── wordService.ts
    │   ├── quizService.ts
    │   └── storyService.ts
    │
    ├── store/               # Zustand store'ları
    │   ├── authStore.ts
    │   └── quizStore.ts
    │
    ├── hooks/               # Özel hook'lar
    │   ├── useAuth.ts
    │   └── useWords.ts
    │
    └── types/               # TypeScript tipleri (packages/shared'dan)
        └── index.ts
```

---

## 🚀 Geliştirme Ortamı Kurulumu

### Ön Koşullar

- [**Node.js 18+**](https://nodejs.org/)
- [**Expo CLI**](https://docs.expo.dev/get-started/installation/)
  ```bash
  npm install -g expo-cli
  ```
- **iOS testi için:** macOS + Xcode (iOS Simulator) veya fiziksel iPhone + Expo Go uygulaması
- **Android testi için:** Android Studio (Android Emulator) veya fiziksel Android cihaz + Expo Go uygulaması

### 1. Bağımlılıkları Yükleyin

Monorepo kök dizininde:

```bash
npm install
```

### 2. Ortam Değişkenlerini Ayarlayın

`apps/mobil/` dizininde `.env` dosyası oluşturun (aşağıya bakınız).

### 3. Geliştirme Sunucusunu Başlatın

```bash
# Monorepo kökünden
npx expo start --workspace=mobile

# Veya apps/mobil/ dizinindeyken
npx expo start
```

Expo Developer Tools açıldıktan sonra:
- **`i`** tuşu → iOS Simulator
- **`a`** tuşu → Android Emulator
- **QR kod** → Fiziksel cihazda Expo Go uygulaması

---

## 🔑 Ortam Değişkenleri (`.env`)

`apps/mobil/` dizinine `.env` dosyası oluşturun. Bu dosya **asla Git'e eklenmemelidir**.

> ⚠️ **Expo'da ortam değişkenleri:** Expo Managed Workflow'da değişkenlere `EXPO_PUBLIC_` prefix'i ile erişilir (`process.env.EXPO_PUBLIC_*`). Gizli değer içeren değişkenleri (API key'ler) **istemci tarafına gömmekten kaçının** — bunları backend üzerinden kullanın.

```env
# Backend API'nin temel URL'i
EXPO_PUBLIC_API_BASE_URL=https://192.168.1.100:5001/api
# Dikkat: Yerel geliştirmede localhost yerine bilgisayarınızın yerel IP'sini kullanın.

# Firebase Konfigürasyonu
# Firebase konsolundan: Proje Ayarları > Genel > Web Uygulamanız (veya Android/iOS uygulaması)
EXPO_PUBLIC_FIREBASE_API_KEY=your-firebase-api-key
EXPO_PUBLIC_FIREBASE_AUTH_DOMAIN=your-project-id.firebaseapp.com
EXPO_PUBLIC_FIREBASE_PROJECT_ID=your-project-id
EXPO_PUBLIC_FIREBASE_STORAGE_BUCKET=your-project-id.appspot.com
EXPO_PUBLIC_FIREBASE_MESSAGING_SENDER_ID=123456789012
EXPO_PUBLIC_FIREBASE_APP_ID=1:123456789012:web:abcdef1234567890
```

---

## 🗺️ Navigasyon Yapısı

```
RootNavigator
│
├── AuthNavigator (oturum açılmamış)
│   └── Stack: LoginScreen
│
└── AppNavigator (oturum açılmış)
    └── BottomTabNavigator
        ├── DashboardScreen   (🏠 Ana Sayfa)
        ├── WordListScreen    (📚 Kelimelerim)
        │   └── Stack: AddWordScreen
        ├── QuizScreen        (🧠 Tekrar)
        └── StoryScreen       (📖 Hikaye)
```

---

## 📡 API İstekleri & Axios Interceptor

Tüm API istekleri `src/services/axiosInstance.ts` üzerinden yapılmalıdır. Interceptor, Firebase Auth'tan güncel JWT token'ı alıp her isteğe otomatik ekler.

```typescript
// src/services/axiosInstance.ts
import axios from 'axios';
import { auth } from '../firebase';
import * as SecureStore from 'expo-secure-store';

const axiosInstance = axios.create({
  baseURL: process.env.EXPO_PUBLIC_API_BASE_URL,
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
      // authStore üzerinden navigasyonu LoginScreen'e yönlendir
    }
    return Promise.reject(error);
  }
);

export default axiosInstance;
```

> 🚫 **Yasaklı:** Component veya screen içlerinde manuel token yönetimi yapmayın. Her zaman `axiosInstance` kullanın.

---

## 🗂️ State Yönetimi (Zustand)

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
  queue: QuizItem[];         // Tekrar edilecek kelime kuyruğu
  currentIndex: number;
  sessionResults: Result[];  // Oturum içi cevap geçmişi
  setQueue: (items: QuizItem[]) => void;
  nextCard: () => void;
  submitAnswer: (quality: number) => void;
  resetSession: () => void;
}
```

---

## 🔒 Kimlik Doğrulama Akışı (Firebase)

```
Kullanıcı → LoginScreen
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
authStore güncellenir → Kullanıcı AppNavigator'a yönlendirilir
```

---

## 📋 Mevcut Ekranlar

| Ekran | Navigasyon Konumu | Açıklama | Auth |
|---|---|---|---|
| `LoginScreen` | AuthStack | E-posta/şifre ile giriş | ❌ |
| `DashboardScreen` | Tab - Ana Sayfa | Streak, günlük hedef, özet | ✅ |
| `WordListScreen` | Tab - Kelimelerim | Kelime listesi ve silme | ✅ |
| `AddWordScreen` | WordList Stack | Yeni kelime ekleme formu | ✅ |
| `QuizScreen` | Tab - Tekrar | Aralıklı tekrar quiz akışı | ✅ |
| `StoryScreen` | Tab - Hikaye | Gemini ile hikaye üretme | ✅ |

---

## 🏗️ Derleme & Yayınlama (EAS Build)

Üretim (production) sürümü oluşturmak için [Expo Application Services (EAS)](https://docs.expo.dev/build/introduction/) kullanılır.

```bash
# EAS CLI yükle
npm install -g eas-cli

# EAS hesabınıza giriş yapın
eas login

# Android APK / AAB oluştur
eas build --platform android --profile production

# iOS IPA oluştur (Apple Developer hesabı gerektirir)
eas build --platform ios --profile production

# Her iki platform aynı anda
eas build --platform all --profile production
```

> 📝 Derleme profilleri `eas.json` dosyasında tanımlanmıştır.

---

*Son güncelleme: Mart 2026*