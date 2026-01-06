import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
  Alert,
  Modal,
  ScrollView,
} from 'react-native';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../navigation/types';
import { Player, KanjiElement } from '../types';
import { getRandomTopicExcluding } from '../data/topics';
import KanjiCanvas from '../components/KanjiCanvas';
import KanjiSelector from '../components/KanjiSelector';
import AnswerInput from '../components/AnswerInput';

type Props = NativeStackScreenProps<RootStackParamList, 'Game'>;

export default function GameScreen({ navigation, route }: Props) {
  const { players: initialPlayers, maxRounds } = route.params;

  const [players, setPlayers] = useState<Player[]>(initialPlayers);
  const [currentRound, setCurrentRound] = useState(1);
  const [guesserIndex, setGuesserIndex] = useState(0);
  const [topic, setTopic] = useState('');
  const [usedTopics, setUsedTopics] = useState<string[]>([]);
  const [kanjiElements, setKanjiElements] = useState<KanjiElement[]>([]);
  const [phase, setPhase] = useState<'hint' | 'answer'>('hint');
  const [showTopic, setShowTopic] = useState(true);
  const [answer, setAnswer] = useState('');

  useEffect(() => {
    startNewRound();
  }, []);

  const startNewRound = () => {
    const newTopic = getRandomTopicExcluding(usedTopics);
    setTopic(newTopic);
    setUsedTopics([...usedTopics, newTopic]);
    setKanjiElements([]);
    setPhase('hint');
    setShowTopic(true);
    setAnswer('');
  };

  const addKanjiElement = (kanji: string) => {
    if (kanjiElements.length >= 8) {
      Alert.alert('制限', '漢字は8文字までです');
      return;
    }

    const newElement: KanjiElement = {
      id: Date.now().toString(),
      kanji,
      x: 150,
      y: 200,
      size: 40,
      rotation: 0,
    };

    setKanjiElements([...kanjiElements, newElement]);
  };

  const updateKanjiElement = (id: string, updates: Partial<KanjiElement>) => {
    setKanjiElements(
      kanjiElements.map(el => (el.id === id ? { ...el, ...updates } : el))
    );
  };

  const removeKanjiElement = (id: string) => {
    setKanjiElements(kanjiElements.filter(el => el.id !== id));
  };

  const finishHint = () => {
    if (kanjiElements.length === 0) {
      Alert.alert('エラー', '最低1文字は漢字を配置してください');
      return;
    }
    setPhase('answer');
    setShowTopic(false);
  };

  const submitAnswer = () => {
    const isCorrect = answer.trim() === topic;

    if (isCorrect) {
      // 当てる側: 1pt
      const updatedPlayers = players.map((p, idx) => {
        if (idx === guesserIndex) {
          return { ...p, score: p.score + 1 };
        }
        // 当てさせる側: 2pt
        return { ...p, score: p.score + 2 };
      });
      setPlayers(updatedPlayers);

      Alert.alert('正解！', `お題: ${topic}\n\n次のラウンドへ`, [
        { text: 'OK', onPress: nextRound },
      ]);
    } else {
      Alert.alert('不正解', `正解: ${topic}\n\n次のラウンドへ`, [
        { text: 'OK', onPress: nextRound },
      ]);
    }
  };

  const nextRound = () => {
    if (currentRound >= maxRounds) {
      // ゲーム終了
      navigation.replace('Result', { players });
    } else {
      // 次のラウンド
      setCurrentRound(currentRound + 1);
      setGuesserIndex((guesserIndex + 1) % players.length);
      startNewRound();
    }
  };

  const guesser = players[guesserIndex];
  const hintCreators = players.filter((_, idx) => idx !== guesserIndex);

  return (
    <View style={styles.container}>
      {/* ヘッダー */}
      <View style={styles.header}>
        <Text style={styles.headerText}>
          ラウンド {currentRound} / {maxRounds}
        </Text>
        <Text style={styles.headerText}>当てる人: {guesser.name}</Text>
      </View>

      {/* お題表示モーダル */}
      <Modal visible={showTopic && phase === 'hint'} transparent animationType="fade">
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={styles.modalTitle}>お題</Text>
            <Text style={styles.topicText}>{topic}</Text>
            <Text style={styles.modalSubtitle}>
              {guesser.name}さん以外の人が確認してください
            </Text>
            <TouchableOpacity
              style={styles.modalButton}
              onPress={() => setShowTopic(false)}
            >
              <Text style={styles.modalButtonText}>OK</Text>
            </TouchableOpacity>
          </View>
        </View>
      </Modal>

      {/* ヒント作成フェーズ */}
      {phase === 'hint' && (
        <View style={styles.content}>
          <View style={styles.infoBox}>
            <Text style={styles.infoText}>
              {hintCreators.map(p => p.name).join('、')}さん:{'\n'}
              漢字を使ってヒントを作成してください
            </Text>
            <Text style={styles.kanjiCount}>
              {kanjiElements.length} / 8 文字
            </Text>
          </View>

          <KanjiCanvas
            elements={kanjiElements}
            onUpdateElement={updateKanjiElement}
            onRemoveElement={removeKanjiElement}
          />

          <KanjiSelector onSelectKanji={addKanjiElement} />

          <TouchableOpacity style={styles.finishButton} onPress={finishHint}>
            <Text style={styles.finishButtonText}>ヒント完成</Text>
          </TouchableOpacity>
        </View>
      )}

      {/* 回答フェーズ */}
      {phase === 'answer' && (
        <View style={styles.content}>
          <View style={styles.infoBox}>
            <Text style={styles.infoText}>
              {guesser.name}さん、お題を当ててください！
            </Text>
          </View>

          <KanjiCanvas
            elements={kanjiElements}
            onUpdateElement={updateKanjiElement}
            onRemoveElement={removeKanjiElement}
            readOnly
          />

          <AnswerInput answer={answer} onChangeAnswer={setAnswer} />

          <TouchableOpacity style={styles.submitButton} onPress={submitAnswer}>
            <Text style={styles.submitButtonText}>回答する</Text>
          </TouchableOpacity>
        </View>
      )}

      {/* スコアボード */}
      <View style={styles.scoreboard}>
        <Text style={styles.scoreboardTitle}>スコア</Text>
        <ScrollView horizontal showsHorizontalScrollIndicator={false}>
          {players.map((player, idx) => (
            <View
              key={player.id}
              style={[
                styles.scoreItem,
                idx === guesserIndex && styles.scoreItemActive,
              ]}
            >
              <Text style={styles.scorePlayerName}>{player.name}</Text>
              <Text style={styles.scorePlayerScore}>{player.score}</Text>
            </View>
          ))}
        </ScrollView>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  header: {
    backgroundColor: '#2196F3',
    padding: 15,
    alignItems: 'center',
  },
  headerText: {
    color: 'white',
    fontSize: 16,
    fontWeight: 'bold',
  },
  content: {
    flex: 1,
    padding: 15,
  },
  infoBox: {
    backgroundColor: 'white',
    borderRadius: 8,
    padding: 15,
    marginBottom: 15,
  },
  infoText: {
    fontSize: 16,
    color: '#333',
    textAlign: 'center',
  },
  kanjiCount: {
    fontSize: 14,
    color: '#666',
    textAlign: 'center',
    marginTop: 5,
  },
  finishButton: {
    backgroundColor: '#4CAF50',
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 15,
  },
  finishButtonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
  },
  submitButton: {
    backgroundColor: '#FF9800',
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 15,
  },
  submitButtonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
  },
  scoreboard: {
    backgroundColor: 'white',
    padding: 15,
    borderTopWidth: 1,
    borderTopColor: '#ddd',
  },
  scoreboardTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 10,
  },
  scoreItem: {
    backgroundColor: '#f5f5f5',
    borderRadius: 8,
    padding: 10,
    marginRight: 10,
    alignItems: 'center',
    minWidth: 80,
  },
  scoreItemActive: {
    backgroundColor: '#FFE082',
  },
  scorePlayerName: {
    fontSize: 14,
    color: '#333',
    fontWeight: 'bold',
  },
  scorePlayerScore: {
    fontSize: 20,
    color: '#2196F3',
    fontWeight: 'bold',
    marginTop: 5,
  },
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0, 0, 0, 0.7)',
    alignItems: 'center',
    justifyContent: 'center',
  },
  modalContent: {
    backgroundColor: 'white',
    borderRadius: 15,
    padding: 30,
    alignItems: 'center',
    minWidth: 300,
  },
  modalTitle: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 20,
  },
  topicText: {
    fontSize: 48,
    fontWeight: 'bold',
    color: '#2196F3',
    marginBottom: 20,
  },
  modalSubtitle: {
    fontSize: 14,
    color: '#666',
    textAlign: 'center',
    marginBottom: 20,
  },
  modalButton: {
    backgroundColor: '#4CAF50',
    paddingHorizontal: 40,
    paddingVertical: 12,
    borderRadius: 25,
  },
  modalButtonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
  },
});
