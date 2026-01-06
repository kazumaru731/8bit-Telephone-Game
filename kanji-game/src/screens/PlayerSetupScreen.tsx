import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  ScrollView,
  Alert,
} from 'react-native';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../navigation/types';
import { Player } from '../types';

type Props = NativeStackScreenProps<RootStackParamList, 'PlayerSetup'>;

export default function PlayerSetupScreen({ navigation }: Props) {
  const [players, setPlayers] = useState<Player[]>([
    { id: '1', name: '', score: 0 },
    { id: '2', name: '', score: 0 },
  ]);
  const [rounds, setRounds] = useState<number>(3);

  const addPlayer = () => {
    const newPlayer: Player = {
      id: Date.now().toString(),
      name: '',
      score: 0,
    };
    setPlayers([...players, newPlayer]);
  };

  const removePlayer = (id: string) => {
    if (players.length <= 2) {
      Alert.alert('エラー', '最低2人必要です');
      return;
    }
    setPlayers(players.filter(p => p.id !== id));
  };

  const updatePlayerName = (id: string, name: string) => {
    setPlayers(players.map(p => (p.id === id ? { ...p, name } : p)));
  };

  const startGame = () => {
    // 名前の入力チェック
    const emptyNames = players.filter(p => !p.name.trim());
    if (emptyNames.length > 0) {
      Alert.alert('エラー', '全員の名前を入力してください');
      return;
    }

    // ラウンド数のチェック
    if (rounds < 1 || rounds > 10) {
      Alert.alert('エラー', 'ラウンド数は1〜10の間で設定してください');
      return;
    }

    navigation.navigate('Game', {
      players,
      maxRounds: rounds,
    });
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.contentContainer}>
      <Text style={styles.title}>プレイヤー設定</Text>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>プレイヤー（2人以上）</Text>
        {players.map((player, index) => (
          <View key={player.id} style={styles.playerRow}>
            <Text style={styles.playerNumber}>{index + 1}.</Text>
            <TextInput
              style={styles.input}
              placeholder="名前を入力"
              value={player.name}
              onChangeText={name => updatePlayerName(player.id, name)}
            />
            {players.length > 2 && (
              <TouchableOpacity
                style={styles.removeButton}
                onPress={() => removePlayer(player.id)}
              >
                <Text style={styles.removeButtonText}>✕</Text>
              </TouchableOpacity>
            )}
          </View>
        ))}

        <TouchableOpacity style={styles.addButton} onPress={addPlayer}>
          <Text style={styles.addButtonText}>+ プレイヤーを追加</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>ラウンド数</Text>
        <View style={styles.roundsContainer}>
          <TouchableOpacity
            style={styles.roundButton}
            onPress={() => setRounds(Math.max(1, rounds - 1))}
          >
            <Text style={styles.roundButtonText}>−</Text>
          </TouchableOpacity>
          <Text style={styles.roundsText}>{rounds}</Text>
          <TouchableOpacity
            style={styles.roundButton}
            onPress={() => setRounds(Math.min(10, rounds + 1))}
          >
            <Text style={styles.roundButtonText}>+</Text>
          </TouchableOpacity>
        </View>
      </View>

      <TouchableOpacity style={styles.startButton} onPress={startGame}>
        <Text style={styles.startButtonText}>ゲーム開始</Text>
      </TouchableOpacity>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  contentContainer: {
    padding: 20,
    paddingBottom: 40,
  },
  title: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 30,
    textAlign: 'center',
  },
  section: {
    marginBottom: 30,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 15,
  },
  playerRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 10,
  },
  playerNumber: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#666',
    width: 30,
  },
  input: {
    flex: 1,
    backgroundColor: 'white',
    borderRadius: 8,
    padding: 12,
    fontSize: 16,
    borderWidth: 1,
    borderColor: '#ddd',
  },
  removeButton: {
    marginLeft: 10,
    width: 36,
    height: 36,
    borderRadius: 18,
    backgroundColor: '#ff5252',
    alignItems: 'center',
    justifyContent: 'center',
  },
  removeButtonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
  },
  addButton: {
    backgroundColor: '#2196F3',
    padding: 12,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 10,
  },
  addButtonText: {
    color: 'white',
    fontSize: 16,
    fontWeight: 'bold',
  },
  roundsContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
  },
  roundButton: {
    width: 50,
    height: 50,
    borderRadius: 25,
    backgroundColor: '#2196F3',
    alignItems: 'center',
    justifyContent: 'center',
  },
  roundButtonText: {
    color: 'white',
    fontSize: 24,
    fontWeight: 'bold',
  },
  roundsText: {
    fontSize: 32,
    fontWeight: 'bold',
    color: '#333',
    marginHorizontal: 30,
  },
  startButton: {
    backgroundColor: '#4CAF50',
    padding: 15,
    borderRadius: 25,
    alignItems: 'center',
    marginTop: 20,
  },
  startButtonText: {
    color: 'white',
    fontSize: 20,
    fontWeight: 'bold',
  },
});
