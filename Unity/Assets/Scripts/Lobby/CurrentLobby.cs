using System.Collections;
using Schema;
using TMPro;
using UnityEngine;

public class CurrentLobby : MonoBehaviour
{
    private GameManager _gameManager;
    private Game _game;
    public SocketIo socketIo;
    public GameObject waitingText;
    public GameObject bottomBar;
    
    private void Start()
    {
        socketIo.PlayerJoined += SocketIoOnPlayerJoined;
        socketIo.PlayerLeft += SocketIoOnPlayerLeft;
        socketIo.GameStarted += SocketIoOnGameStarted;
        
        _gameManager = GameManager.Instance;
        _game = _gameManager.Game;

        UpdateWaitingText();
        UpdateBottomBar();
        StartCoroutine(StartGame());
    }

    private void SocketIoOnPlayerJoined(Player player)
    {
        UpdateWaitingText();
        UpdateBottomBar();
    }
    
    private void SocketIoOnPlayerLeft(Player player)
    {
        UpdateWaitingText();
        UpdateBottomBar();
    }
    
    private void SocketIoOnGameStarted()
    {
        StartCoroutine(StartGame());
    }

    private void UpdateWaitingText()
    {
        var game = GameManager.Instance.Game;
        var waitingTextTransform = waitingText.transform;
        var waitingTextMeshPro = waitingTextTransform.GetComponent<TextMeshProUGUI>();
        waitingTextMeshPro.text = "Waiting for players " + game.Players.Count + "/" + game.Seats;
    }
    
    private void UpdateBottomBar()
    {
        var game = GameManager.Instance.Game;
        var selfPlayerId = AuthenticationManager.Instance.user.Id;
        var bottomBarTransform = bottomBar.transform;

        if (game == null) return;

        for (var i = 0; i < 4; i++)
        {
            var nameTextTransform = bottomBarTransform.GetChild(i).GetChild(0);
            var typeTextTransform = bottomBarTransform.GetChild(i).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var typeTextMeshPro = typeTextTransform.GetComponent<TextMeshProUGUI>();

            nameTextMeshPro.text = "";
            typeTextMeshPro.text = "";
        }

        foreach (var player in game.Players)
        {
            var index = player.Index;
        
            var nameTextTransform = bottomBarTransform.GetChild(index).GetChild(0);
            var typeTextTransform = bottomBarTransform.GetChild(index).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var typeTextMeshPro = typeTextTransform.GetComponent<TextMeshProUGUI>();

            if (player.UserId == selfPlayerId)
            {
                nameTextMeshPro.text = "•" + player.Name + "•";
            }
            else
            {
                nameTextMeshPro.text = player.Name;
            }
            
            typeTextMeshPro.text = "Player";
        }
    }

    private IEnumerator StartGame()
    {
        if (_game.Status != "running") yield break;

        var waitingTextTransform = waitingText.transform;
        var waitingTextMeshPro = waitingTextTransform.GetComponent<TextMeshProUGUI>();
            
        for (var i = 3; i >= 0; i--)
        {
            waitingTextMeshPro.text = "Game starting in " + i;
            yield return new WaitForSecondsRealtime(1f);
        }
            
        _gameManager.GoToGame(_game);
    }
}
