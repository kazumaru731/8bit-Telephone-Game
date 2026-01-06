import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet, ScrollView } from 'react-native';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../navigation/types';

type Props = NativeStackScreenProps<RootStackParamList, 'Result'>;

export default function ResultScreen({ navigation, route }: Props) {
  const { players } = route.params;

  // スコア順にソート
  const sortedPlayers = [...players].sort((a, b) => b.score - a.score);
  const winner = sortedPlayers[0];

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.contentContainer}>
      <Text style={styles.title}>ゲーム終了！</Text>

      <View style={styles.winnerBox}>
        <Text style={styles.winnerLabel}>優勝</Text>
        <Text style={styles.winnerName}>{winner.name}</Text>
        <Text style={styles.winnerScore}>{winner.score} pt</Text>
      </View>

      <View style={styles.rankingContainer}>
        <Text style={styles.rankingTitle}>最終結果</Text>
        {sortedPlayers.map((player, index) => (
          <View
            key={player.id}
            style={[
              styles.rankItem,
              index === 0 && styles.rankItemFirst,
              index === 1 && styles.rankItemSecond,
              index === 2 && styles.rankItemThird,
            ]}
          >
            <View style={styles.rankLeft}>
              <Text style={styles.rankNumber}>{index + 1}</Text>
              <Text style={styles.rankName}>{player.name}</Text>
            </View>
            <Text style={styles.rankScore}>{player.score} pt</Text>
          </View>
        ))}
      </View>

      <TouchableOpacity
        style={styles.playAgainButton}
        onPress={() => navigation.navigate('PlayerSetup')}
      >
        <Text style={styles.playAgainButtonText}>もう一度遊ぶ</Text>
      </TouchableOpacity>

      <TouchableOpacity
        style={styles.homeButton}
        onPress={() => navigation.navigate('Home')}
      >
        <Text style={styles.homeButtonText}>ホームに戻る</Text>
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
    alignItems: 'center',
  },
  title: {
    fontSize: 32,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 30,
    marginTop: 20,
  },
  winnerBox: {
    backgroundColor: '#FFD700',
    borderRadius: 15,
    padding: 30,
    alignItems: 'center',
    width: '100%',
    maxWidth: 400,
    marginBottom: 30,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 6,
    elevation: 5,
  },
  winnerLabel: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#8B4513',
    marginBottom: 10,
  },
  winnerName: {
    fontSize: 36,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 10,
  },
  winnerScore: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#FF6B6B',
  },
  rankingContainer: {
    width: '100%',
    maxWidth: 400,
    backgroundColor: 'white',
    borderRadius: 15,
    padding: 20,
    marginBottom: 30,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  rankingTitle: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 15,
    textAlign: 'center',
  },
  rankItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: '#f9f9f9',
    borderRadius: 8,
    padding: 15,
    marginBottom: 10,
  },
  rankItemFirst: {
    backgroundColor: '#FFE082',
  },
  rankItemSecond: {
    backgroundColor: '#E0E0E0',
  },
  rankItemThird: {
    backgroundColor: '#FFCCBC',
  },
  rankLeft: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  rankNumber: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#666',
    marginRight: 15,
    width: 30,
  },
  rankName: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#333',
  },
  rankScore: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#2196F3',
  },
  playAgainButton: {
    backgroundColor: '#4CAF50',
    paddingHorizontal: 40,
    paddingVertical: 15,
    borderRadius: 25,
    marginBottom: 15,
    width: '100%',
    maxWidth: 300,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.2,
    shadowRadius: 4,
    elevation: 3,
  },
  playAgainButtonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
    textAlign: 'center',
  },
  homeButton: {
    backgroundColor: '#9E9E9E',
    paddingHorizontal: 40,
    paddingVertical: 15,
    borderRadius: 25,
    width: '100%',
    maxWidth: 300,
  },
  homeButtonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
    textAlign: 'center',
  },
});
