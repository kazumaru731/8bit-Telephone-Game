export interface Player {
  id: string;
  name: string;
  score: number;
}

export interface KanjiElement {
  id: string;
  kanji: string;
  x: number;
  y: number;
  size: number;
  rotation: number;
}

export interface Round {
  roundNumber: number;
  topic: string;
  guesserId: string;
  kanjiElements: KanjiElement[];
  answer: string | null;
  isCorrect: boolean | null;
}

export interface GameState {
  players: Player[];
  currentRound: number;
  maxRounds: number;
  rounds: Round[];
  currentGuesserId: string | null;
}
