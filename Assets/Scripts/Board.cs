using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 盤面クラス
public class Board : MonoBehaviour {

    // const
    private const float FillPieceDuration = 0.2f;
    private const float SwitchPieceCuration = 0.02f;

    // serialize field.
    [SerializeField]
    private GameObject piecePrefab;
    [SerializeField]
    private TweenAnimationManager animManager;
    [SerializeField]
    private UIManager uiManager;

    // public
    public int pieceWidth;

    // private.
    private Piece[,] board;
    private int width;
    private int height;
    private int randomSeed;
    private Vector2[] directions = new Vector2[] { Vector2.up, Vector2.down, Vector2.right, Vector2.left };
    private List<AnimData> fillPieceAnim = new List<AnimData>();
    private List<Vector2> pieceCreatePos = new List<Vector2>();

    //-------------------------------------------------------
    // Public Function
    //-------------------------------------------------------
    // 特定の幅と高さに盤面を初期化する
    public void InitializeBoard(int boardWidth, int boardHeight)
    {
        width = boardWidth;
        height = boardHeight;

        pieceWidth = Screen.width / boardWidth;

        board = new Piece[width, height];
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                CreatePiece(new Vector2(i, j));
            }
        }

        animManager.AddListAnimData(fillPieceAnim);
    }

    // 入力されたクリック(タップ)位置から最も近いピースの位置を返す
    public Piece GetNearestPiece(Vector3 input)
    {
        var x = Mathf.Min((int)(input.x / pieceWidth), width - 1);
        var y = Mathf.Min((int)(input.y / pieceWidth), height - 1);
        return board[x, y];
    }

    // 盤面上のピースを交換する
    public void SwitchPiece(Piece p1, Piece p2)
    {
        // 位置を移動する
        var animList = new List<AnimData>();
        animList.Add(new AnimData(p1.gameObject, GetPieceWorldPos(GetPieceBoardPos(p2)), SwitchPieceCuration));
        animList.Add(new AnimData(p2.gameObject, GetPieceWorldPos(GetPieceBoardPos(p1)), SwitchPieceCuration));
        animManager.AddListAnimData(animList);

        // 盤面データを更新する
        var p1BoardPos = GetPieceBoardPos(p1);
        var p2BoardPos = GetPieceBoardPos(p2);
        board[(int)p1BoardPos.x, (int)p1BoardPos.y] = p2;
        board[(int)p2BoardPos.x, (int)p2BoardPos.y] = p1;
    }

    // 盤面上にマッチングしているピースがあるかどうかを判断する
    public bool HasMatch()
    {
        foreach (var piece in board)
        {
            if (IsMatchPiece(piece))
            {
                return true;
            }
        }
        return false;
    }

    // マッチングしているピースを削除する
    public IEnumerator DeleteMatchPiece(Action endCallBadk)
    {
        foreach (var piece in board)
        {
            if (piece != null && IsMatchPiece(piece))
            {
                var pos = GetPieceBoardPos(piece);
                DestroyMatchPiece(pos, piece.GetKind());
                uiManager.AddCombo();
                yield return new WaitForSeconds(0.4f);
            }
        }
        endCallBadk();
    }

    // ピースが消えている場所を詰めて、新しいピースを生成する
    public IEnumerator FillPiece(Action endCallBack)
    {
        // アニメーション管理リストとピース生成位置保持リストを初期化する
        fillPieceAnim.Clear();
        pieceCreatePos.Clear();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                FillPiece(new Vector2(i, j));
            }
        }

        // アニメーションを再生する
        animManager.AddListAnimData(fillPieceAnim);

        yield return new WaitForSeconds(1f);
        endCallBack();
    }

    // ピースのオブジェクトを生成する
    public Piece InstantiatePiece(Vector3 createPos)
    {
        var piece = Instantiate(piecePrefab, createPos, Quaternion.identity).GetComponent<Piece>();
        piece.transform.SetParent(transform);
        return piece;
    }

    //-------------------------------------------------------
    // Private Function
    //-------------------------------------------------------
    // 特定の位置にピースを作成する
    private void CreatePiece(Vector2 position)
    {
        // ピースの位置を求める
        var piecePos = GetPieceWorldPos(position);

        // ピースの生成位置を求める
        var createPos = new Vector2(position.x, height);
        while (pieceCreatePos.Contains(createPos))
        {
            createPos += Vector2.up;
        }

        pieceCreatePos.Add(createPos);
        var pieceCreateWorldPos = GetPieceWorldPos(createPos);

        // ピースを生成、ボードの子オブジェクトにする
        var piece = InstantiatePiece(pieceCreateWorldPos);
        piece.SetSize(pieceWidth);

        // 生成するピースの種類をランダムに決める
        var kind = (PieceKind)UnityEngine.Random.Range(0, Enum.GetNames(typeof(PieceKind)).Length);
        piece.SetKind(kind);

        // 盤面にピースの情報をセットする
        board[(int)position.x, (int)position.y] = piece;

        // アニメーションのセット
        fillPieceAnim.Add(new AnimData(piece.gameObject, piecePos, FillPieceDuration));
    }

    // 盤面上の位置からピースオブジェクトのワールド座標での位置を返す
    private Vector3 GetPieceWorldPos(Vector2 boardPos)
    {
        return new Vector3(boardPos.x* pieceWidth + (pieceWidth / 2), boardPos.y* pieceWidth + (pieceWidth / 2), 0);
    }

    // ピースが盤面上のどの位置にあるのかを返す
    private Vector2 GetPieceBoardPos(Piece piece)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (board[i, j] == piece)
                {
                    return new Vector2(i, j);
                }
            }
        }

        return Vector2.zero;
    }

    // 対象のピースがマッチしているかの判定を行う
    private bool IsMatchPiece(Piece piece)
    {
        // ピースの情報を取得
        var pos = GetPieceBoardPos(piece);
        var kind = piece.GetKind();

        // 縦方向にマッチするかの判定 MEMO: 自分自身をカウントするため +1 する
        var verticalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.up) + GetSameKindPieceNum(kind, pos, Vector2.down) + 1;

        // 横方向にマッチするかの判定 MEMO: 自分自身をカウントするため +1 する
        var horizontalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.right) + GetSameKindPieceNum(kind, pos, Vector2.left) + 1;

        return verticalMatchCount >= GameManager.MachingCount || horizontalMatchCount >= GameManager.MachingCount;
    }

    // 対象の方向に引数で指定したの種類のピースがいくつあるかを返す
    private int GetSameKindPieceNum(PieceKind kind, Vector2 piecePos, Vector2 searchDir)
    {
        var count = 0;
        while (true)
        {
            piecePos += searchDir;
            if (IsInBoard(piecePos) && board[(int)piecePos.x, (int)piecePos.y].GetKind() == kind)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    // 対象の座標がボードに存在するか(ボードからはみ出していないか)を判定する
    private bool IsInBoard(Vector2 pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;
    }

    // 特定のピースのが削除されているかを判断し、削除されているなら詰めるか、それができなければ新しく生成する
    private void FillPiece(Vector2 pos)
    {
        var piece = board[(int)pos.x, (int)pos.y];
        if (piece != null && !piece.deleteFlag)
        {
            // ピースが削除されていなければ何もしない
            return;
        }

        // 対象のピースより上方向に有効なピースがあるかを確認、あるなら場所を移動させる
        var checkPos = pos + Vector2.up;
        while (IsInBoard(checkPos))
        {
            var checkPiece = board[(int)checkPos.x, (int)checkPos.y];
            if (checkPiece != null && !checkPiece.deleteFlag)
            {
                fillPieceAnim.Add(new AnimData(checkPiece.gameObject, GetPieceWorldPos(pos), FillPieceDuration));
                board[(int)pos.x, (int)pos.y] = checkPiece;
                board[(int)checkPos.x, (int)checkPos.y] = null;
                return;
            }
            checkPos += Vector2.up;
        }

        // 有効なピースがなければ新しく作る
        CreatePiece(pos);
    }

    // 特定のピースがマッチしている場合、ほかのマッチしたピースとともに削除する
    private void DestroyMatchPiece(Vector2 pos, PieceKind kind)
    {
        // ピースの場所が盤面以外だったら何もしない
        if (!IsInBoard(pos))
        {
            return;
        }

        // ピースが無効であったり削除フラグが立っていたりそもそも、種別がちがうならば何もしない
        var piece = board[(int)pos.x, (int)pos.y];
        if (piece == null || piece.deleteFlag || piece.GetKind() != kind)
        {
            return;
        }

        // ピースが同じ種類でもマッチングしてなければ何もしない
        if (!IsMatchPiece(piece))
        {
            return;
        }

        // 削除フラグをたてて、周り４方のピースを判定する
        piece.deleteFlag = true;
        foreach (var dir in directions)
        {
            DestroyMatchPiece(pos + dir, kind);
        }

        // ピースを削除する
        var tweenAlpha = piece.gameObject.AddComponent<AlphaTween>();
        tweenAlpha.DoTween(1, 0, 0.3f, () => Destroy(piece.gameObject));
    }
}
