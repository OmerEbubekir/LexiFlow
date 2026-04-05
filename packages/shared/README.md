# @lexiflow/shared

LexiFlow ekosisteminin (Backend, Web ve Mobil) ortak kalbidir. Bu paket içerisinde, projenin farklı katmanlarında veri bütünlüğünü sağlamak adına temel DTO'lar (Data Transfer Objects) ve bir API İstemcisi barındırılır.

## Özellikler

1. **DTO Eşleşmesi (C# ile %100 Uyum):** 
   Backend'in kullandığı tüm Request ve Response modelleri (camelCase formatında) `WordDto`, `WordleGameDto`, vb. bu pakette deklare edilmiştir. Front-end projelerimiz bu interface'leri doğrudan import eder ve veri bozulmalarının önüne geçer.
2. **Otomatik Yetkilendirme (Interceptor):**
   Uygulama genelinde kullanılmak üzere Axios tabanlı bir `apiClient` sunar. Bu istemci arka planda `firebase/auth` kullanarak açık oturum olan kullanıcının güncel token'ını edinir ve otomatik olarak `Authorization: Bearer <token>` header'ını ekler.

## Kurulum ve Kullanım

Bu paket monorepo (workspaces) dahilinde projelere bağlanmıştır. React veya React Native projenizde şu şekilde kullanılabilir:

### 1. API İstekleri Yapmak (apiClient)

`apiClient` tüm Firebase Auth interceptor ayarlamalarını kendi içinde otomatik yapar. Ekstra token yönetimine gerek duymazsınız.

```typescript
import { apiClient } from '@lexiflow/shared';

// Örnek: Kullanıcının kelimelerini çekmek
async function fetchWords() {
  try {
    const response = await apiClient.get('/Words?page=1&pageSize=10');
    console.log("Kelimeler:", response.data.data.items);
  } catch (error) {
    console.error("Hata:", error);
  }
}
```

### 2. TypeScript Tip Koruması Kullanmak (DTOs)

```typescript
import { WordDto, AddWordRequest, BaseResponse } from '@lexiflow/shared';

const newWord: AddWordRequest = {
  englishWord: 'crane',
  turkishTranslation: 'vinç',
  difficultyLevel: 1,
  samples: [
    {
      sentenceText: 'I saw a huge crane.',
      turkishTranslation: 'Büyük bir vinç gördüm.'
    }
  ]
};

async function addWord() {
  const response = await apiClient.post<BaseResponse<string>>('/Words', newWord);
  if (response.data.success) {
    console.log("Kelime Eklendi! ID:", response.data.data);
  }
}
```

## Yapı

- `/src/types/index.ts` -> Tüm DTO arayüzleri
- `/src/api/apiClient.ts` -> Axios tabanlı hazır HTTP client
- `/src/index.ts` -> Dışa aktarım ana dosyası
