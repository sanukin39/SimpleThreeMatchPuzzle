using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ゲーム管理クラス
public class GameManager : MonoBehaviour {

    // const.
    public const int MachingCount = 3;
    private const float SelectedPieceAlpha = 0.5f;

    // enum.
    private enum GameState
    {
        Idle,
        PieceMove,
        MatchCheck,
        DeletePiece,
        FillPiece,
        Wait,
    }

    // serialize field.
    [SerializeField]
    private Board board;
    [SerializeField]
    private UIManager uiManager;

    // private.
    private GameState currentState;
    private Piece selectedPiece;
    private GameObject selectedPieceObject;

     //-------------------------------------------------------
    // MonoBehaviour Function
    //-------------------------------------------------------
    // ゲームの初期化処理
    private void Start()
    {
        Application.targetFrameRate = 60;

        board.InitializeBoard(6, 5);

        currentState = GameState.Idle;
    }

    // ゲームのメインループ
    private void Update()
    {
        switch (currentState)
        {
            case GameState.Idle:
                Idle();
                break;
            case GameState.PieceMove:
                PieceMove();
                break;
            case GameState.MatchCheck:
                MatchCheck();
                break;
            case GameState.DeletePiece:
                DeletePiece();
                break;
            case GameState.FillPiece:
                FillPiece();
                break;
            case GameState.Wait:
                break;
            default:
                break;
        }
        uiManager.SetStatusText(currentState.ToString());
    }

    //-------------------------------------------------------
    // Private Function
    //-------------------------------------------------------
    // プレイヤーの入力を検知し、ピースを選択状態にする
    private void Idle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            uiManager.ResetCombo();
            SelectPiece();
        }
    }

    // プレイヤーがピースを選択しているときの処理、入力終了を検知したら盤面のチェックの状態に移行する
    private void PieceMove()
    {
        if (Input.GetMouseButton(0))
        {
            var piece = board.GetNearestPiece(Input.mousePosition);
            if (piece != selectedPiece)
            {
                board.SwitchPiece(selectedPiece, piece);
            }
            selectedPieceObject.transform.position = Input.mousePosition + Vector3.up * 10;
        }
        else if (Input.GetMouseButtonUp(0)) {
            selectedPiece.SetPieceAlpha(1f);
            Destroy(selectedPieceObject);
            currentState = GameState.MatchCheck;
        }
    }

    // 盤面上にマッチングしているピースがあるかどうかを判断する
    private void MatchCheck()
    {
        if (board.HasMatch())
        {
            currentState = GameState.DeletePiece;
        }
        else
        {
            currentState = GameState.Idle;
        }
    }

    // マッチングしているピースを削除する
    private void DeletePiece()
    {
        currentState = GameState.Wait;
        StartCoroutine(board.DeleteMatchPiece(() => currentState = GameState.FillPiece));
    }

    // 盤面上のかけている部分にピースを補充する
    private void FillPiece()
    {
        currentState = GameState.Wait;
        StartCoroutine(board.FillPiece(() => currentState = GameState.MatchCheck));
    }

    // ピースを選択する処理
    private void SelectPiece()
    {
        selectedPiece = board.GetNearestPiece(Input.mousePosition);
        var piece = board.InstantiatePiece(Input.mousePosition);
        piece.SetKind(selectedPiece.GetKind());
        piece.SetSize((int)(board.pieceWidth * 1.2f));
        piece.SetPieceAlpha(SelectedPieceAlpha);
        selectedPieceObject = piece.gameObject;

        selectedPiece.SetPieceAlpha(SelectedPieceAlpha);
        currentState = GameState.PieceMove;
    }
}
