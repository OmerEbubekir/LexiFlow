# 📦 LearnLanguage — Shared Package (Paylaşılan TypeScript Tipleri)

> Bu paket, `apps/web` ve `apps/mobil` arasında ortak kullanılan TypeScript tip tanımlarını (DTO'lar, arayüzler) ve yardımcı fonksiyonları barındırır. Tek kaynak gerçeği (Single Source of Truth) prensibi ile çalışır; tüm API veri modelleri buradan yayımlanır.

---

## 📑 İçindekiler

- [Klasör Yapısı](#-klasör-yapısı)
- [Kullanım](#-kullanım)
- [Tip Tanımları](#-tip-tanımları)
- [Yardımcı Fonksiyonlar](#️-yardımcı-fonksiyonlar)
- [Yeni Tip / Util Ekleme](#-yeni-tip--util-ekleme)

---

## 📂 Klasör Yapısı

```
packages/shared/
├── types/
│   ├── api.ts        # Genel API yanıt wrapper tipleri
│   ├── word.ts       # Kelime ile ilgili tüm tipler
│   ├── quiz.ts       # Quiz ile ilgili tipler
│   ├── story.ts      # Hikaye ile ilgili tipler
│   └── index.ts      # Tüm tiplerin tek noktadan export'u
│
└── utils/
    ├── dateUtils.ts  # Tarih/saat yardımcıları (UTC dönüşümleri)
    ├── sm2.ts        # SM-2 Spaced Repetition algoritması
    └── index.ts      # Tüm utility'lerin export'u
```

---

## 📌 Kullanım

```typescript
// Web veya Mobile içinden import
import type { Word, QuizItem, ApiResponse } from '@learnlanguage/shared/types';
import { calculateNextReview } from '@learnlanguage/shared/utils';
```

---

## 🧩 Tip Tanımları

### `api.ts` — Genel API Yanıt Yapıları

Tüm backend yanıtları bu wrapper tipler üzerinden modellenir.

```typescript
// Başarılı tekil yanıt
export interface ApiResponse<T> {
  success: true;
  data: T;
}

// Başarılı sayfalandırılmış liste yanıtı
export interface PaginatedResponse<T> {
  success: true;
  data: {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
  };
}

// RFC 7807 — Hata yanıtı
export interface ApiError {
  type: string;
  title: string;
  status: number;
  detail: string;
  instance: string;
  errors?: Record<string, string[]>;
}
```

---

### `word.ts` — Kelime Tipleri

```typescript
export type SupportedLanguage = 'en' | 'de' | 'fr' | 'es' | 'it' | 'tr';

// Backend'den gelen tam kelime nesnesi
export interface Word {
  id: string;                   // UUID
  word: string;
  translation: string;
  exampleSentence?: string;
  language: SupportedLanguage;
  easeFactor: number;           // SM-2: Başlangıç 2.5
  interval: number;             // SM-2: Gün cinsinden tekrar aralığı
  repetitions: number;          // SM-2: Başarılı tekrar sayısı
  nextReviewDate: string;       // ISO 8601 UTC (örn: "2026-03-10T00:00:00Z")
  createdAt: string;            // ISO 8601 UTC
}

// Yeni kelime eklemek için kullanılan DTO
export interface CreateWordDto {
  word: string;
  translation: string;
  exampleSentence?: string;
  language: SupportedLanguage;
}

// Kelime güncellemek için kullanılan DTO
export interface UpdateWordDto {
  translation?: string;
  exampleSentence?: string;
}
```

---

### `quiz.ts` — Quiz Tipleri

```typescript
// Tekrar ekranında gösterilen kelime kartı
export interface QuizItem {
  wordId: string;
  word: string;
  translation: string;
  exampleSentence?: string;
}

// Quiz cevabı göndermek için kullanılan DTO
// quality: 0–5 (SM-2 kalite puanı)
//   0–2: Yanlış / hatırlanamadı
//   3:   Zor hatırlandı
//   4:   Doğru cevap
//   5:   Çok kolay
export interface SubmitAnswerDto {
  wordId: string;
  quality: 0 | 1 | 2 | 3 | 4 | 5;
}

// Cevap gönderilince dönen SM-2 güncelleme sonucu
export interface AnswerResult {
  wordId: string;
  newInterval: number;
  newEaseFactor: number;
  nextReviewDate: string;  // ISO 8601 UTC
}
```

---

### `story.ts` — Hikaye Tipleri

```typescript
export type StoryLength = 'short' | 'medium' | 'long';

// Hikaye üretimi için DTO
export interface GenerateStoryDto {
  wordIds: string[];
  language: SupportedLanguage;
  storyLength: StoryLength;
}

// Üretilen hikaye yanıtı
export interface GeneratedStory {
  story: string;
  wordsUsed: string[];
  generatedAt: string;  // ISO 8601 UTC
}
```

---

## 🛠️ Yardımcı Fonksiyonlar

### `dateUtils.ts` — Tarih Yardımcıları

```typescript
/**
 * ISO 8601 UTC tarih stringini kullanıcının yerel
 * zaman dilimine göre formatlar.
 * @param isoString - "2026-03-10T00:00:00Z"
 * @returns "10 Mart 2026"
 */
export function formatReviewDate(isoString: string): string;

/**
 * Bir tarihin bugünden kaç gün sonra olduğunu döner.
 * Geçmiş tarihler için negatif değer döner.
 */
export function daysFromNow(isoString: string): number;

/**
 * Belirtilen aralık (gün) eklenerek yeni UTC tarih üretir.
 */
export function addDays(isoString: string, days: number): string;
```

---

### `sm2.ts` — SM-2 Algoritması

Aralıklı tekrar hesaplaması için kullanılan SM-2 algoritmasının istemci tarafı implementasyonu. Backend ile aynı mantığı paylaşır.

```typescript
export interface SM2Input {
  quality: 0 | 1 | 2 | 3 | 4 | 5;
  easeFactor: number;
  interval: number;
  repetitions: number;
}

export interface SM2Output {
  newEaseFactor: number;
  newInterval: number;
  newRepetitions: number;
}

/**
 * SM-2 algoritmasını uygular ve güncellenmiş değerleri döner.
 * Kalite < 3 ise interval ve repetitions sıfırlanır.
 */
export function calculateNextReview(input: SM2Input): SM2Output;
```

---

## ➕ Yeni Tip / Util Ekleme

1. İlgili dosyayı (`types/` veya `utils/`) düzenleyin.
2. Yeni export'u `types/index.ts` veya `utils/index.ts` dosyasına ekleyin.
3. Web ve Mobile tarafında kullanmadan önce TypeScript derlemesini doğrulayın:
   ```bash
   # Monorepo kökünden
   npx tsc --noEmit
   ```

> ⚠️ Bu pakette **hiçbir platform-specific (React Native veya browser-only) API kullanılmaz.** Kod hem Node.js hem de React Native ortamında çalışabilir olmalıdır.

---

*Son güncelleme: Mart 2026*
