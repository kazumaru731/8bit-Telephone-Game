import React from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  PanResponder,
  Dimensions,
} from 'react-native';
import { KanjiElement } from '../types';

interface Props {
  elements: KanjiElement[];
  onUpdateElement: (id: string, updates: Partial<KanjiElement>) => void;
  onRemoveElement: (id: string) => void;
  readOnly?: boolean;
}

const CANVAS_WIDTH = Dimensions.get('window').width - 30;
const CANVAS_HEIGHT = 300;

export default function KanjiCanvas({
  elements,
  onUpdateElement,
  onRemoveElement,
  readOnly = false,
}: Props) {
  const createPanResponder = (element: KanjiElement) => {
    if (readOnly) return null;

    return PanResponder.create({
      onStartShouldSetPanResponder: () => true,
      onPanResponderMove: (_, gestureState) => {
        const newX = Math.max(
          0,
          Math.min(CANVAS_WIDTH - element.size, element.x + gestureState.dx)
        );
        const newY = Math.max(
          0,
          Math.min(CANVAS_HEIGHT - element.size, element.y + gestureState.dy)
        );

        onUpdateElement(element.id, {
          x: newX,
          y: newY,
        });
      },
    });
  };

  return (
    <View style={styles.container}>
      <View style={styles.canvas}>
        {elements.map(element => {
          const panResponder = createPanResponder(element);

          return (
            <View
              key={element.id}
              style={[
                styles.kanjiContainer,
                {
                  left: element.x,
                  top: element.y,
                  transform: [{ rotate: `${element.rotation}deg` }],
                },
              ]}
              {...(panResponder ? panResponder.panHandlers : {})}
            >
              <Text style={[styles.kanjiText, { fontSize: element.size }]}>
                {element.kanji}
              </Text>

              {!readOnly && (
                <View style={styles.controls}>
                  <TouchableOpacity
                    style={styles.controlButton}
                    onPress={() =>
                      onUpdateElement(element.id, {
                        size: Math.max(20, element.size - 10),
                      })
                    }
                  >
                    <Text style={styles.controlButtonText}>−</Text>
                  </TouchableOpacity>

                  <TouchableOpacity
                    style={styles.controlButton}
                    onPress={() =>
                      onUpdateElement(element.id, {
                        size: Math.min(100, element.size + 10),
                      })
                    }
                  >
                    <Text style={styles.controlButtonText}>+</Text>
                  </TouchableOpacity>

                  <TouchableOpacity
                    style={styles.controlButton}
                    onPress={() =>
                      onUpdateElement(element.id, {
                        rotation: (element.rotation - 45) % 360,
                      })
                    }
                  >
                    <Text style={styles.controlButtonText}>↺</Text>
                  </TouchableOpacity>

                  <TouchableOpacity
                    style={[styles.controlButton, styles.deleteButton]}
                    onPress={() => onRemoveElement(element.id)}
                  >
                    <Text style={styles.controlButtonText}>✕</Text>
                  </TouchableOpacity>
                </View>
              )}
            </View>
          );
        })}

        {elements.length === 0 && !readOnly && (
          <View style={styles.emptyState}>
            <Text style={styles.emptyStateText}>
              下から漢字を選んで配置してください
            </Text>
          </View>
        )}
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    marginBottom: 15,
  },
  canvas: {
    width: CANVAS_WIDTH,
    height: CANVAS_HEIGHT,
    backgroundColor: 'white',
    borderRadius: 8,
    borderWidth: 2,
    borderColor: '#2196F3',
    position: 'relative',
  },
  kanjiContainer: {
    position: 'absolute',
  },
  kanjiText: {
    fontWeight: 'bold',
    color: '#333',
  },
  controls: {
    flexDirection: 'row',
    marginTop: 5,
  },
  controlButton: {
    backgroundColor: '#2196F3',
    width: 28,
    height: 28,
    borderRadius: 14,
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: 5,
  },
  deleteButton: {
    backgroundColor: '#ff5252',
  },
  controlButtonText: {
    color: 'white',
    fontSize: 14,
    fontWeight: 'bold',
  },
  emptyState: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
  emptyStateText: {
    fontSize: 16,
    color: '#999',
  },
});
