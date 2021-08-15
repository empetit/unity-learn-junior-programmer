using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{

    [SerializeField]
    private GameObject DefaultScreen;
    
    [SerializeField]
    private GameObject LoadingScreen;

    [SerializeField]
    private TMPro.TMP_InputField _userNameField;
    
    [SerializeField]
    private TMPro.TMP_Text _progressLabel;
    
    [SerializeField]
    private TMPro.TMP_Text _bestScore;

    [SerializeField]
    private int _sceneIndex;
    private bool _levelReady;
    private MainManager _gameManager;
    public int bestScoreValue = 0;
    public string bestScoreUser;
    public string currentUserName;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        DefaultScreen.SetActive(true);
        LoadingScreen.SetActive(false);

        this.currentUserName = PlayerPrefs.GetString("lastUserName", "NewPlayer");
        _userNameField.text = this.currentUserName;

        this.bestScoreUser = PlayerPrefs.GetString("bestScoreUser", "");
        this.bestScoreValue = PlayerPrefs.GetInt("bestScoreValue", 0);

        if( bestScoreValue > 0 ){
            this._bestScore.text = "HighScore : " + this.bestScoreUser + " : " + this.bestScoreValue + " pts";
        }else{
            this._bestScore.text = "";
        }
    }

    void Update(){
        if(_levelReady){
            if(Input.GetKeyDown(KeyCode.KeypadEnter)){
                this._gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainManager>();
                LoadingScreen.SetActive(false);
            }
        }
    }

    public void SetUsername( string userName){
        this.currentUserName = userName;
    }

    public void LoadNextLevel(){
        
        PlayerPrefs.SetString("lastUserName", this.currentUserName);
        
        StartCoroutine(LoadNextLevelAsync());
    }

    IEnumerator LoadNextLevelAsync(){
        _levelReady = false;

        DefaultScreen.SetActive(false);
        LoadingScreen.SetActive(true);
        
        var loading = SceneManager.LoadSceneAsync(_sceneIndex);

        while( loading.progress < 1 ){
            this._progressLabel.text = Mathf.Round(loading.progress * 100) + "%";
            yield return new WaitForEndOfFrame();
        }
            this._progressLabel.text = "Press <Enter> to continue";
        
        _levelReady = true;
    }

    internal bool UpdateScores(int m_Points)
    {
        if(this.bestScoreValue < m_Points){
            
            this.bestScoreUser = this.currentUserName;
            this.bestScoreValue = m_Points;

            PlayerPrefs.SetString("bestScoreUser", this.currentUserName);
            PlayerPrefs.SetInt("bestScoreValue", m_Points);

            return true;
        }else{
            return false;
        }
    }
}
