// 1. Base Response
export interface BaseResponse<T = any> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
}

// 2. Words
export interface WordSampleDto {
  id: string;
  sentenceText: string;
  turkishTranslation: string;
}

export interface WordDto {
  id: string;
  englishWord: string;
  turkishTranslation: string;
  pictureUrl?: string;
  audioUrl?: string;
  difficultyLevel: number;
  categoryId?: string;
  samples: WordSampleDto[];
}

export interface AddWordSampleRequest {
  sentenceText: string;
  turkishTranslation: string;
}

export interface AddWordRequest {
  englishWord: string;
  turkishTranslation: string;
  pictureUrl?: string;
  audioUrl?: string;
  difficultyLevel: number;
  categoryId?: string;
  samples: AddWordSampleRequest[];
}

export interface UpdateWordRequest {
  englishWord: string;
  turkishTranslation: string;
  pictureUrl?: string;
  audioUrl?: string;
  difficultyLevel: number;
  categoryId?: string;
  samples: WordSampleDto[];
}

// 3. Quiz (6-Rep Spaced Repetition)
export interface ReviewWordDto {
  userWordProgressId: string;
  word: WordDto;
  consecutiveCorrect: number;
  nextReviewDate: string;
}

export interface SubmitAnswerRequest {
  wordId: string;
  isCorrect: boolean;
}

// 4. Wordle
export interface WordleGameDto {
  gameId: string;
  targetWordLength: number;
  maxAttempts: number;
}

export interface WordleGuessRequest {
  guess: string;
}

export interface WordleGuessResult {
  isWon: boolean;
  isGameOver: boolean;
  patterns: string[]; // (🟩, 🟨, ⬛)
}

// 5. Story
export interface GenerateStoryRequest {
  wordIds: string[];
  language: string;
}

export interface StoryResponse {
  storyText: string;
  generatedAt: string;
}
