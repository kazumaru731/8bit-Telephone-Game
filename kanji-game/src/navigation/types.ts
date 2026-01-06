import { Player } from '../types';

export type RootStackParamList = {
  Home: undefined;
  PlayerSetup: undefined;
  Game: {
    players: Player[];
    maxRounds: number;
  };
  Result: {
    players: Player[];
  };
};
