import React from 'react';
import { View, TextInput, StyleSheet } from 'react-native';

interface Props {
  answer: string;
  onChangeAnswer: (answer: string) => void;
}

export default function AnswerInput({ answer, onChangeAnswer }: Props) {
  return (
    <View style={styles.container}>
      <TextInput
        style={styles.input}
        placeholder="答えを入力してください"
        value={answer}
        onChangeText={onChangeAnswer}
        autoCapitalize="none"
        autoCorrect={false}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    marginBottom: 15,
  },
  input: {
    backgroundColor: 'white',
    borderRadius: 8,
    padding: 15,
    fontSize: 18,
    borderWidth: 2,
    borderColor: '#FF9800',
  },
});
