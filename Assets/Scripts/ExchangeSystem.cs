using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeSystem : MonoBehaviour
{
    #region 교환시스템 변수

    // =====================
    [Space(30)] // UI 관련 변수
                // =====================

    // 교환 단계(스테이지)의 위치를 담는 리스트
    [SerializeField] private List<Transform> exchangeStageTransform = new List<Transform>();

    // 선택된 텍스트 그룹의 위치를 나타내는 트랜스폼
    [SerializeField] private Transform selectTextGroup;

    // 에러 발생 시 표시되는 패널
    [SerializeField] private GameObject settingsError_Panel;

    // 프로필 이미지 UI
    [SerializeField] private Image profile_Image;

    // 닉네임 텍스트 UI
    [SerializeField] private TMP_Text nickname_Text;

    [Header("Pico Coin")]
    public TMP_Text picoCoinValue;

    [Header("Glitter Crystal")]
    public TMP_Text glitterCrystalValue;

    [Header("Brick Coin")]
    public TMP_Text brickCoinValue;

    [Header("QuantityDesignation")]
    [SerializeField] private TMP_Text inputCurrency_Text;
    [SerializeField] private TMP_Text outputCurrency_Text;

    [SerializeField] private TMP_Text inputExchangeRateCurrency_Text;
    [SerializeField] private TMP_Text outputExchangeRateCurrency_Text;

    [SerializeField] private TMP_Text inputFeeRateCurrency_Text;
    [SerializeField] private TMP_Text outputFeeRateCurrency_Text;

    [SerializeField] private TMP_Text refreshTable_Text;

    [SerializeField] private TMP_Text myCurrencyName_Text;
    [SerializeField] private TMP_Text myCurrencyPoint_Text;

    [SerializeField] private TMP_Text exchangeCurrencyName_Text;
    [SerializeField] private TMP_InputField exchangeCurrencyAmount_InputField;

    [SerializeField] private TMP_Text checkExchange_Text;
    [SerializeField] private TMP_Text checkExchangeValue_Text;

    // =====================
    [Space(30)] // 상호작용 관련 변수
    // =====================

    // 현재 선택된 스테이지 번호
    [SerializeField] private int currentStageNumber;

    // 현재 스테이지에서 선택된 블루 라인 오브젝트
    [SerializeField] private GameObject currentStageblueLine;

    // 자원 입력 패널 오브젝트
    [SerializeField] private GameObject inputResourcePanel;

    // 현재 선택된 입력 & 출력 화폐
    [SerializeField] private int seletedInputCurrency;
    [SerializeField] private int seletedOutputCurrency;
    [SerializeField] private int exchangeAmoutCurrency;

    // 수량 지정 패널 오브젝트
    [SerializeField] private GameObject quantityDesignationPanel;

    // =====================
    // 기타 상태 변수
    // =====================

    #endregion

    private void OnEnable()
    {
        // 프로필과 닉네임이 설정이 되어 있지 않으면 패널이 열리지 않고 오류 메시지를 출력하는 에러 발생시키는 코드
        #region 프로필과 닉네임 설정 여부

        if (PlayerWalletManager.Instance.profile == null || string.IsNullOrEmpty(PlayerWalletManager.Instance.nickname))
        {
            onTriggerExchangeSettingsErrorNotify();
            gameObject.SetActive(false);

            Debug.LogError("시스템 예외 상황 : 프로필과 닉네임 세팅을 해야 합니다.");
        }
        else if (PlayerWalletManager.Instance.profile != null && !string.IsNullOrEmpty(PlayerWalletManager.Instance.nickname))
        {
            Debug.Log($"프로필이 존재하고 닉네임 또한 Null이 아니므로 패널이 열립니다.");

            StartCoroutine(StartSetupCoroutine());  // 패널 자식 오브젝트 세팅을 위해 패널이 열릴 때까지 기다린 후 세팅을 시작하는 코루틴
        }

        #endregion

        #region 홈 화면 세팅

        InvokeRepeating("PriceTrendCalculation", 0, 10);

        #endregion
    }

    private IEnumerator StartSetupCoroutine()
    {
        yield return new WaitUntil(() => gameObject.activeSelf);
        Debug.Log("시작 세팅 코루틴을 시작합니다.");

        foreach (Transform childTransform in selectTextGroup)
        {
            exchangeStageTransform.Add(childTransform);
        }

        currentStageNumber = 0;
        currentStageblueLine.transform.localPosition = new Vector3(exchangeStageTransform[currentStageNumber].localPosition.x, currentStageblueLine.transform.localPosition.y);

        inputResourcePanel.SetActive(true);
        quantityDesignationPanel.SetActive(false);

        profile_Image.sprite = PlayerWalletManager.Instance.profile;
        nickname_Text.text = PlayerWalletManager.Instance.nickname;
    }   // 패널이 열릴 때까지 대기를 한 후 초기 환경세팅을 시작하는 코루틴

    private void onTriggerExchangeSettingsErrorNotify()
    {
        StartCoroutine(ExchangePanelErrorNotify());
    }   // 사용자에게 오류의 내용을 알려주는 코루틴을 관리하는 매서드

    private IEnumerator ExchangePanelErrorNotify()
    {
        settingsError_Panel.SetActive(true);
        settingsError_Panel.GetComponent<Animator>().SetTrigger("isError");

        yield return null;

        //Animator animator = settingsError_Panel.GetComponent<Animator>();
        //while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        //{
        //	Debug.Log(animator.GetCurrentAnimatorStateInfo(0));
        //	Debug.Log("현재 반환 중");
        //	yield return null;
        //}

        //Debug.Log("세팅 에러 패널 초기화");
        //settingsError_Panel.SetActive(false);
        //settingsPersonalInformationCoroutine = null;
    }   // 오류 내용을 출력하는 코루틴 매서드

    private Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform childObject in parent)
        {
            if (childObject.name == childName)
            {
                return childObject;
            }

            Transform foundChildObject = FindChildRecursive(childObject, childName);

            if (foundChildObject != null)
            {
                return foundChildObject;
            }
        }

        return null;
    }   // 자식 객체 안의 자식 객체를 찾기 위한 매서드

    private void PriceTrendCalculation()
    {
        if (!inputResourcePanel.activeInHierarchy)
        {
            return;
        }

        #region 피코 코인

        float _tempPicoCoinFloat = 0;

        foreach (float picoCoinExchangeRate in ExchangeManager.Instance.picoCoinTrends)
        {
            _tempPicoCoinFloat += picoCoinExchangeRate;
        }

        float _tempCalculatedPicoCoin = _tempPicoCoinFloat / ExchangeManager.Instance.picoCoinTrends.Count;

        if (Mathf.Abs(_tempCalculatedPicoCoin - ExchangeManager.Instance.originCoinExchangeRate) <= 0.5f)
        {
            Debug.Log($"피코 코인의 값이 {_tempCalculatedPicoCoin}이므로 +- 0.5포인트 범위 내에 있습니다.");
            picoCoinValue.text = "<color=yellow> 가치에 변동이 거의 없습니다. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedPicoCoin - ExchangeManager.Instance.originCoinExchangeRate) <= 0.5f) && _tempCalculatedPicoCoin - ExchangeManager.Instance.originCoinExchangeRate > 0)
        {
            Debug.Log($"피코 코인의 값이 {_tempCalculatedPicoCoin}이므로 현재 상승 중 입니다.");
            picoCoinValue.text = "<color=#fc4c4e> 가치 상승 중 입니다. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedPicoCoin - ExchangeManager.Instance.originCoinExchangeRate) <= 0.5f) && _tempCalculatedPicoCoin - ExchangeManager.Instance.originCoinExchangeRate < 0)
        {
            Debug.Log($"피코 코인의 값이 {_tempCalculatedPicoCoin}이므로 현재 하락 중 입니다.");
            picoCoinValue.text = "<color=#0c43b7> 가치 하락 중 입니다. </color>";
        }
        else
        {
            picoCoinValue.text = "<color=grey> 가치 판단 중 입니다. </color>";
        }

        #endregion

        #region 글리터 크리스탈

        float _tempGlitterFloat = 0;

        foreach (float GlitterCrystalExchangeRate in ExchangeManager.Instance.glitterCrystalTrends)
        {
            _tempGlitterFloat += GlitterCrystalExchangeRate;
        }

        float _tempCalculatedGlitterCrystal = _tempGlitterFloat / ExchangeManager.Instance.glitterCrystalTrends.Count;

        if (Mathf.Abs(_tempCalculatedGlitterCrystal - ExchangeManager.Instance.originCrystalExchangeRate) <= 0.5f)
        {
            Debug.Log($"글리터 크리스탈의 값이 {_tempCalculatedGlitterCrystal}이므로 +- 0.5포인트 범위 내에 있습니다.");
            glitterCrystalValue.text = "<color=yellow> 가치에 변동이 거의 없습니다. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedGlitterCrystal - ExchangeManager.Instance.originCrystalExchangeRate) <= 0.5f) && _tempCalculatedGlitterCrystal - ExchangeManager.Instance.originCrystalExchangeRate > 0)
        {
            Debug.Log($"글리터 크리스탈의 값이 {_tempCalculatedGlitterCrystal}이므로 현재 상승 중 입니다.");
            glitterCrystalValue.text = "<color=#fc4c4e> 가치 상승 중 입니다. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedGlitterCrystal - ExchangeManager.Instance.originCrystalExchangeRate) <= 0.5f) && _tempCalculatedGlitterCrystal - ExchangeManager.Instance.originCrystalExchangeRate < 0)
        {
            Debug.Log($"글리터 크리스탈의 값이 {_tempCalculatedGlitterCrystal}이므로 현재 하락 중 입니다.");
            glitterCrystalValue.text = "<color=#0c43b7> 가치 하락 중 입니다. </color>";
        }
        else
        {
            glitterCrystalValue.text = "<color=grey> 가치 판단 중 입니다. </color>";
        }

        #endregion

        #region 브릭 코인

        float _tempBrickCoinFloat = 0;

        foreach (float brickCoinExchangeRate in ExchangeManager.Instance.brickCoinTrends)
        {
            _tempBrickCoinFloat += brickCoinExchangeRate;
        }

        float _tempCalculatedbrickCoin = _tempBrickCoinFloat / ExchangeManager.Instance.brickCoinTrends.Count;

        if (Mathf.Abs(_tempCalculatedbrickCoin - ExchangeManager.Instance.originBrickExchangeRate) <= 0.5f)
        {
            Debug.Log($"브릭 코인의 값이 {_tempCalculatedbrickCoin}이므로 +- 0.5포인트 범위 내에 있습니다.");
            brickCoinValue.text = "<color=yellow> 가치에 변동이 거의 없습니다. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedbrickCoin - ExchangeManager.Instance.originBrickExchangeRate) <= 0.5f) && _tempCalculatedbrickCoin - ExchangeManager.Instance.originBrickExchangeRate > 0)
        {
            Debug.Log($"브릭 코인의 값이 {_tempCalculatedbrickCoin}이므로 현재 상승 중 입니다.");
            brickCoinValue.text = "<color=#fc4c4e> 가치 상승 중 입니다. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedbrickCoin - ExchangeManager.Instance.originBrickExchangeRate) <= 0.5f) && _tempCalculatedbrickCoin - ExchangeManager.Instance.originBrickExchangeRate < 0)
        {
            Debug.Log($"브릭 코인의 값이 {_tempCalculatedbrickCoin}이므로 현재 하락 중 입니다.");
            brickCoinValue.text = "<color=#0c43b7> 가치 하락 중 입니다. </color>";
        }
        else
        {
            brickCoinValue.text = "<color=grey> 가치 판단 중 입니다. </color>";
        }

        Debug.Log("계산 완료");
        #endregion
    }

    public void OnClickCurrencyButton(int currencyNumber)
    {
        if (currentStageNumber == 0)
        {
            seletedInputCurrency = currencyNumber;

            currentStageNumber++;
            currentStageblueLine.transform.localPosition = new Vector3(exchangeStageTransform[currentStageNumber].localPosition.x, currentStageblueLine.transform.localPosition.y);

            quantityDesignationPanel.SetActive(false);
            inputResourcePanel.SetActive(true);
        }
        else if (currentStageNumber == 1)
        {
            seletedOutputCurrency = currencyNumber;

            currentStageNumber++;
            currentStageblueLine.transform.localPosition = new Vector3(exchangeStageTransform[currentStageNumber].localPosition.x, currentStageblueLine.transform.localPosition.y);

            inputResourcePanel.SetActive(false);
            quantityDesignationPanel.SetActive(true);

            SetupQuantityDesignationPanel();
        }

    }   // inputResource에서 화폐 버튼 클릭 시 발동하는 매서드

    private void SetupQuantityDesignationPanel()
    {
        refreshTable_Text.text = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}에 새로고침";

        switch (seletedInputCurrency)
        {
            case 0:
                inputCurrency_Text.text = "피코 코인(PCC)";
                inputExchangeRateCurrency_Text.text = ExchangeManager.Instance.coinExchangeRate.ToString("F4");
                myCurrencyName_Text.text = "나의 피코 코인(PCC)";
                myCurrencyPoint_Text.text = PlayerWalletManager.Instance.TotalCoin.ToString("#,##0");
                inputFeeRateCurrency_Text.text = ExchangeManager.Instance.coinFeeRate.ToString("F2");
                break;
            case 1:
                inputCurrency_Text.text = "글리터 크리스탈(GLTC)";
                inputExchangeRateCurrency_Text.text = ExchangeManager.Instance.crystalExchangeRate.ToString("F4");
                myCurrencyName_Text.text = "나의 글리터 크리스탈(GLTC)";
                myCurrencyPoint_Text.text = PlayerWalletManager.Instance.TotalCrystal.ToString("#,##0");
                inputFeeRateCurrency_Text.text = ExchangeManager.Instance.crystalFeeRate.ToString("F2");
                break;
            case 2:
                inputCurrency_Text.text = "브릭 코인(BC)";
                inputExchangeRateCurrency_Text.text = ExchangeManager.Instance.brickExchangeRate.ToString("F4");
                myCurrencyName_Text.text = "나의 브릭 코인(BC)";
                myCurrencyPoint_Text.text = PlayerWalletManager.Instance.TotalBrick.ToString("#,##0");
                inputFeeRateCurrency_Text.text = ExchangeManager.Instance.brickFeeRate.ToString("F2");
                break;
        }

        switch (seletedOutputCurrency)
        {
            case 0:
                outputCurrency_Text.text = "피코 코인(PCC)";
                outputExchangeRateCurrency_Text.text = ExchangeManager.Instance.coinExchangeRate.ToString("F4");
                exchangeCurrencyName_Text.text = "전환 피코 코인(PCC)";
                outputFeeRateCurrency_Text.text = ExchangeManager.Instance.coinFeeRate.ToString("F2");
                break;
            case 1:
                outputCurrency_Text.text = "글리터 크리스탈(GLTC)";
                outputExchangeRateCurrency_Text.text = ExchangeManager.Instance.crystalExchangeRate.ToString("F4");
                exchangeCurrencyName_Text.text = "전환 글리터 크리스탈(GLTC)";
                outputFeeRateCurrency_Text.text = ExchangeManager.Instance.crystalFeeRate.ToString("F2");
                break;
            case 2:
                outputCurrency_Text.text = "브릭 코인(BC)";
                outputExchangeRateCurrency_Text.text = ExchangeManager.Instance.brickExchangeRate.ToString("F4");
                exchangeCurrencyName_Text.text = "전환 브릭 코인(BC)";
                outputFeeRateCurrency_Text.text = ExchangeManager.Instance.brickFeeRate.ToString("F2");
                break;
        }
    }

    public void RefreshQuantityDesignationPanel()
    {
        SetupQuantityDesignationPanel();
    }

    private void Update()
    {
        if (!quantityDesignationPanel.activeInHierarchy)
            return;

        int.TryParse(exchangeCurrencyAmount_InputField.text, out exchangeAmoutCurrency);

        float inputFeeAmount = 0;   // 입력 화폐의 수수료 계산 변수
        float inputFinalFee = 0;    // 입력 화폐의 수수료 계산 후 양 최종 변수

        float finalOutputAmount = 0;    // 출력 화폐의 양 계산 변수

        switch (seletedInputCurrency) 
        {
            case 0:
                inputFeeAmount = exchangeAmoutCurrency * (ExchangeManager.Instance.coinFeeRate / 100);
                inputFinalFee = exchangeAmoutCurrency - inputFeeAmount;
                break;
            case 1:
                inputFeeAmount = exchangeAmoutCurrency * (ExchangeManager.Instance.crystalFeeRate / 100);
                inputFinalFee = exchangeAmoutCurrency - inputFeeAmount;
                break;
            case 2:
                inputFeeAmount = exchangeAmoutCurrency * (ExchangeManager.Instance.brickFeeRate / 100);
                inputFinalFee = exchangeAmoutCurrency - inputFeeAmount;
                break;
        }

        switch (seletedOutputCurrency)
        {
            case 0:
                finalOutputAmount = inputFinalFee * ExchangeManager.Instance.coinExchangeRate;
                break;
            case 1:
                finalOutputAmount = inputFinalFee * ExchangeManager.Instance.crystalExchangeRate;
                break;
            case 2:
                finalOutputAmount = inputFinalFee * ExchangeManager.Instance.brickExchangeRate;
                break;
        }

        checkExchange_Text.text = $"{inputCurrency_Text.text}(을)를 {outputCurrency_Text.text}(으)로 {Mathf.RoundToInt(finalOutputAmount)}만큼 전환한다.";
        checkExchangeValue_Text.text = $"({exchangeAmoutCurrency}) / ({Mathf.RoundToInt(finalOutputAmount)}) / ({Mathf.RoundToInt(inputFeeAmount)})";
    }

    public void OnCalculateButtonClicked()
    {
        int exchangeAmount;

        switch (seletedInputCurrency)
        {
            case 0:
                if (int.TryParse(exchangeCurrencyAmount_InputField.text, out exchangeAmount))
                {
                    if (PlayerWalletManager.Instance.TotalCoin < exchangeAmount)
                    {
                        Debug.LogError($"플레이어 화폐: {PlayerWalletManager.Instance.TotalCoin} 입력 양: {exchangeAmount}");
                        return;
                    }
                }
                break;
            case 1:
                if (int.TryParse(exchangeCurrencyAmount_InputField.text, out exchangeAmount))
                {
                    if (PlayerWalletManager.Instance.TotalCrystal < exchangeAmount)
                    {
                        Debug.LogError($"플레이어 화폐: {PlayerWalletManager.Instance.TotalCoin} 입력 양: {exchangeAmount}");
                        return;
                    }
                }
                break;
            case 2:
                if (int.TryParse(exchangeCurrencyAmount_InputField.text, out exchangeAmount))
                {
                    if (PlayerWalletManager.Instance.TotalBrick < exchangeAmount)
                    {
                        Debug.LogError($"플레이어 화폐: {PlayerWalletManager.Instance.TotalCoin} 입력 양: {exchangeAmount}");
                        return;
                    }
                }
                break;
        }

        int inputAmount = int.Parse(exchangeCurrencyAmount_InputField.text);
        float finalAmount = CalculateFinalExchangeAmount(inputAmount);

        switch (seletedOutputCurrency)
        {
            case 0:
                PlayerWalletManager.Instance.TotalCoin += Mathf.RoundToInt(finalAmount);
                break;
            case 1:
                PlayerWalletManager.Instance.TotalCrystal += Mathf.RoundToInt(finalAmount);
                break;
            case 2:
                PlayerWalletManager.Instance.TotalBrick += Mathf.RoundToInt(finalAmount);
                break;
        }
    }

    private float CalculateFinalExchangeAmount(int inputAmount)
    {
        float inputFeeAmount = 0;   // 입력 화폐의 수수료 계산 변수
        float inputFinalFee = 0;    // 입력 화폐의 수수료 계산 후 양 최종 변수

        float finalOutputAmount = 0;    // 출력 화폐의 양 계산 변수

        switch (seletedInputCurrency)
        {
            case 0:
                inputFeeAmount = inputAmount * (ExchangeManager.Instance.coinFeeRate / 100);
                inputFinalFee = inputAmount - inputFeeAmount;
                PlayerWalletManager.Instance.TotalCoin -= inputAmount;
                break;
            case 1:
                inputFeeAmount = inputAmount * (ExchangeManager.Instance.crystalFeeRate / 100);
                inputFinalFee = inputAmount - inputFeeAmount;
                PlayerWalletManager.Instance.TotalCrystal -= inputAmount;
                break;
            case 2:
                inputFeeAmount = inputAmount * (ExchangeManager.Instance.brickFeeRate / 100);
                inputFinalFee = inputAmount - inputFeeAmount;
                PlayerWalletManager.Instance.TotalBrick -= inputAmount;
                break;
        }

        switch (seletedOutputCurrency)
        {
            case 0:
                finalOutputAmount = inputFinalFee * ExchangeManager.Instance.coinExchangeRate;
                break;
            case 1:
                finalOutputAmount = inputFinalFee * ExchangeManager.Instance.crystalExchangeRate;
                break;
            case 2:
                finalOutputAmount = inputFinalFee * ExchangeManager.Instance.brickExchangeRate;
                break;
        }

        return finalOutputAmount;
    }
}
