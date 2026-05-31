namespace KanjiFlipGame.Core
{
    /// <summary>
    /// プレイヤーの役割を定義する列挙型
    /// </summary>
    public enum PlayerRole
    {
        /// <summary>役割未定</summary>
        None,
        
        /// <summary>出題者（フリップを作成する側）</summary>
        Questioner,
        
        /// <summary>回答者（フリップを見てお題を当てる側）</summary>
        Answerer
    }

    /// <summary>
    /// ゲームの状態を定義する列挙型
    /// </summary>
    public enum GameState
    {
        /// <summary>待機中（マッチング前）</summary>
        Waiting,
        
        /// <summary>出題フェーズ（出題者がフリップを作成中）</summary>
        Questioning,
        
        /// <summary>回答フェーズ（回答者が回答中）</summary>
        Answering,
        
        /// <summary>結果表示中</summary>
        ShowingResult,
        
        /// <summary>ゲーム終了</summary>
        GameOver
    }
}
