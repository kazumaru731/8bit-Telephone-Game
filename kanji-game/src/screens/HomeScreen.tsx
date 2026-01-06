import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../navigation/types';

type Props = NativeStackScreenProps<RootStackParamList, 'Home'>;

export default function HomeScreen({ navigation }: Props) {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>漢字連想ゲーム</Text>
      <Text style={styles.subtitle}>小学1・2年生の漢字で遊ぼう！</Text>

      <View style={styles.rulesContainer}>
        <Text style={styles.rulesTitle}>遊び方</Text>
        <Text style={styles.rulesText}>
          • 2人以上で遊びます{'\n'}
          • 各ラウンド、1人が「当てる人」になります{'\n'}
          • 他の人はお題を見て、漢字でヒントを作ります{'\n'}
          • 漢字は8文字以内で、大きさや位置を自由に配置できます{'\n'}
          • 当てた人: 1ポイント{'\n'}
          • 当てさせた人: 2ポイント{'\n'}
          • 最高得点の人が勝ち！
        </Text>
      </View>

      <TouchableOpacity
        style={styles.startButton}
        onPress={() => navigation.navigate('PlayerSetup')}
      >
        <Text style={styles.startButtonText}>ゲームを始める</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 20,
  },
  title: {
    fontSize: 36,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 10,
  },
  subtitle: {
    fontSize: 18,
    color: '#666',
    marginBottom: 40,
  },
  rulesContainer: {
    backgroundColor: 'white',
    borderRadius: 15,
    padding: 20,
    marginBottom: 40,
    width: '100%',
    maxWidth: 400,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  rulesTitle: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 15,
  },
  rulesText: {
    fontSize: 16,
    color: '#555',
    lineHeight: 24,
  },
  startButton: {
    backgroundColor: '#4CAF50',
    paddingHorizontal: 40,
    paddingVertical: 15,
    borderRadius: 25,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.2,
    shadowRadius: 4,
    elevation: 3,
  },
  startButtonText: {
    color: 'white',
    fontSize: 20,
    fontWeight: 'bold',
  },
});
