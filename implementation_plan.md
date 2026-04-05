# LexiFlow Frontend & Mobil Mimari Yol Haritası (Phases 9-15)

LexiFlow projesi Backend yapısı (Faz 1-8) başarıyla tamamlanmıştır. Uygulamanın Monorepo yapısına (`packages/shared`, `apps/web`, `apps/mobile`) uygun olarak geliştirilecek Frontend adımları aşağıdaki gibi planlanmıştır. 

## Kullanıcı Onayı Bekleniyor (Requesting Review)

Aşağıdaki plan projenin frontend ve mobil iskeletini inşa etme sıralamasını gösterir. Bu sıraya uygun çalışıp çalışmamak konusundaki onayınızı bekliyorum. Onayınızın ardından ilk olarak Phase 9 (`packages/shared`) inşasına başlayacağız.

> [!IMPORTANT]
> Projenin tüm endpointlerini 100% kapsamla test etmiş olsak da Front-end entegrasyonu aşamasında Zustand ve Axios tarafındaki Interceptor mantığı kritik bir güvenlik/data-binding yapısı oluşturacaktır.

---

## Önerilen Aşama Sıralaması (Proposed Roadmap)

### Phase 9: `packages/shared` - Ortak Altyapı
Bu modül **Hem Web Hem Mobil** projelerinin kalbi olacak. Hiçbir framework'e bağımlı olmayan saf TypeScript/JavaScript mantığı burada yer alacak.
- **DTO ve Tipler:** Backend modellerinin TS Interface karşılıkları (`WordDto`, `QuizResponse`, vb).
- **API Client:** Axios tabanlı genel kütüphane ve hata/başarı durumlarının Interceptor üzerinden (Firebase JIT Provisioning ile uyumlu) yönetimi.
- **Merkezi State Logic:** Zustand ile store oluşturma hook'ları (Paylaşılabilir düzeyde olanlar).

### Phase 10: `apps/web` - Core İskelet ve Firebase Auth
Vite temelli uygulamanın ayağa kalkması.
- **Routing:** React Router DOM ayarları.
- **Login/Register:** Firebase üzerinden yetkilendirme akışları + Private Route.
- **Design System:** TailwindCSS ve ana arayüz (UI) konteynerları, navbar/sidebar.

### Phase 11: `apps/web` - Kelime Havuzu & Dashboard
Kullanıcının uygulamada en çok vakit geçireceği yer.
- Eğlenceli Dashboard (Günlük istatistikler ve kısayollar).
- Kelime Ekleme/Düzenleme/Silme özellikleri, tablo ve liste görünümleri.

### Phase 12: `apps/web` - Spaced Repetition (6-Rep) ve Quiz Mekaniği
Backend'de tasvir edilen SM-2 tabanlı 6-Rep algoritmasının kullanıcıya sunumu.
- Günlük tekrar kartları (Tinder swipe veya Flashcard tarzı animasyonlu).
- Kolay/Normal/Zor butonlarıyla Ease Factor'ün güncellenmesi.

### Phase 13: `apps/web` - Gamification (Gemini AI & Wordle)
Öğrenmeyi zevkli kılan özelliklerin arayüze eklenmesi.
- Gemini ile seçilen kelimelerden kısa hikaye üretimi ve okuma tasarımı.
- Sadece tamamen (IsLearned=true) öğrenilmiş 5 harfli kelimelerden oluşan bir "LexiFlow Wordle" mini-oyunu.

### Phase 14 & Phase 15: `apps/mobile` - React Native (Expo)
- Expo Router ve Firebase entegrasyonu.
- `packages/shared` kütüphanesinin mobil tarafa bağlanması.
- Kelime listesi, Swipe Flashcard ve Mini oyun ekranlarının mobil görünüm odaklı inşası.

---

## Onayınızı Bekliyorum
Bu plan projemizin mevcut Backend iş mantığıyla tamamen örtüşmektedir. `packages/shared` kütüphanesini geliştirmeye başlayabilmem için lütfen onaylayın veya planda değiştirmek istediğiniz bir adım varsa belirtin.
