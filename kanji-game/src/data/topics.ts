// ゲームで使用するお題リスト
export const topics = [
  // 動物
  '犬',
  '車',
  '学校',
  '花',
  '本',
  '音楽',
  '食事',
  '家族',
  '友人',
  '先生',
  '雨',
  '雪',
  '太陽',
  '月',
  '星',
  '海',
  '山',
  '川',
  '森',
  '公園',
  '図書館',
  '買い物',
  '料理',
  '運動',
  '野球',
  '絵画',
  '歌',
  '時計',
  '電車',
  '自転車',
  '飛行機',
  '船',
  '橋',
  '道路',
  '信号',
  '病院',
  '郵便局',
  '銀行',
  '駅',
  '空港',
  '動物園',
  '水族館',
  '遊園地',
  '映画館',
  '劇場',
  '美術館',
  '博物館',
  '寺',
  '神社',
  '城'
];

// ランダムにお題を取得
export const getRandomTopic = (): string => {
  const randomIndex = Math.floor(Math.random() * topics.length);
  return topics[randomIndex];
};

// 使用済みのお題を避けてランダムに取得
export const getRandomTopicExcluding = (usedTopics: string[]): string => {
  const availableTopics = topics.filter(topic => !usedTopics.includes(topic));

  if (availableTopics.length === 0) {
    // 全て使い切った場合は全リストからランダムに
    return getRandomTopic();
  }

  const randomIndex = Math.floor(Math.random() * availableTopics.length);
  return availableTopics[randomIndex];
};
