using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeSystem : MonoBehaviour
{
    #region ��ȯ�ý��� ����

    // =====================
    [Space(30)] // UI ���� ����
                // =====================

    // ��ȯ �ܰ�(��������)�� ��ġ�� ��� ����Ʈ
    [SerializeField] private List<Transform> exchangeStageTransform = new List<Transform>();

    // ���õ� �ؽ�Ʈ �׷��� ��ġ�� ��Ÿ���� Ʈ������
    [SerializeField] private Transform selectTextGroup;

    // ���� �߻� �� ǥ�õǴ� �г�
    [SerializeField] private GameObject settingsError_Panel;

    // ������ �̹��� UI
    [SerializeField] private Image profile_Image;

    // �г��� �ؽ�Ʈ UI
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
    [Space(30)] // ��ȣ�ۿ� ���� ����
    // =====================

    // ���� ���õ� �������� ��ȣ
    [SerializeField] private int currentStageNumber;

    // ���� ������������ ���õ� ��� ���� ������Ʈ
    [SerializeField] private GameObject currentStageblueLine;

    // �ڿ� �Է� �г� ������Ʈ
    [SerializeField] private GameObject inputResourcePanel;

    // ���� ���õ� �Է� & ��� ȭ��
    [SerializeField] private int seletedInputCurrency;
    [SerializeField] private int seletedOutputCurrency;
    [SerializeField] private int exchangeAmoutCurrency;

    // ���� ���� �г� ������Ʈ
    [SerializeField] private GameObject quantityDesignationPanel;

    // =====================
    // ��Ÿ ���� ����
    // =====================

    #endregion

    private void OnEnable()
    {
        // �����ʰ� �г����� ������ �Ǿ� ���� ������ �г��� ������ �ʰ� ���� �޽����� ����ϴ� ���� �߻���Ű�� �ڵ�
        #region �����ʰ� �г��� ���� ����

        if (PlayerWalletManager.Instance.profile == null || string.IsNullOrEmpty(PlayerWalletManager.Instance.nickname))
        {
            onTriggerExchangeSettingsErrorNotify();
            gameObject.SetActive(false);

            Debug.LogError("�ý��� ���� ��Ȳ : �����ʰ� �г��� ������ �ؾ� �մϴ�.");
        }
        else if (PlayerWalletManager.Instance.profile != null && !string.IsNullOrEmpty(PlayerWalletManager.Instance.nickname))
        {
            Debug.Log($"�������� �����ϰ� �г��� ���� Null�� �ƴϹǷ� �г��� �����ϴ�.");

            StartCoroutine(StartSetupCoroutine());  // �г� �ڽ� ������Ʈ ������ ���� �г��� ���� ������ ��ٸ� �� ������ �����ϴ� �ڷ�ƾ
        }

        #endregion

        #region Ȩ ȭ�� ����

        InvokeRepeating("PriceTrendCalculation", 0, 10);

        #endregion
    }

    private IEnumerator StartSetupCoroutine()
    {
        yield return new WaitUntil(() => gameObject.activeSelf);
        Debug.Log("���� ���� �ڷ�ƾ�� �����մϴ�.");

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
    }   // �г��� ���� ������ ��⸦ �� �� �ʱ� ȯ�漼���� �����ϴ� �ڷ�ƾ

    private void onTriggerExchangeSettingsErrorNotify()
    {
        StartCoroutine(ExchangePanelErrorNotify());
    }   // ����ڿ��� ������ ������ �˷��ִ� �ڷ�ƾ�� �����ϴ� �ż���

    private IEnumerator ExchangePanelErrorNotify()
    {
        settingsError_Panel.SetActive(true);
        settingsError_Panel.GetComponent<Animator>().SetTrigger("isError");

        yield return null;

        //Animator animator = settingsError_Panel.GetComponent<Animator>();
        //while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        //{
        //	Debug.Log(animator.GetCurrentAnimatorStateInfo(0));
        //	Debug.Log("���� ��ȯ ��");
        //	yield return null;
        //}

        //Debug.Log("���� ���� �г� �ʱ�ȭ");
        //settingsError_Panel.SetActive(false);
        //settingsPersonalInformationCoroutine = null;
    }   // ���� ������ ����ϴ� �ڷ�ƾ �ż���

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
    }   // �ڽ� ��ü ���� �ڽ� ��ü�� ã�� ���� �ż���

    private void PriceTrendCalculation()
    {
        if (!inputResourcePanel.activeInHierarchy)
        {
            return;
        }

        #region ���� ����

        float _tempPicoCoinFloat = 0;

        foreach (float picoCoinExchangeRate in ExchangeManager.Instance.picoCoinTrends)
        {
            _tempPicoCoinFloat += picoCoinExchangeRate;
        }

        float _tempCalculatedPicoCoin = _tempPicoCoinFloat / ExchangeManager.Instance.picoCoinTrends.Count;

        if (Mathf.Abs(_tempCalculatedPicoCoin - ExchangeManager.Instance.originCoinExchangeRate) <= 0.5f)
        {
            Debug.Log($"���� ������ ���� {_tempCalculatedPicoCoin}�̹Ƿ� +- 0.5����Ʈ ���� ���� �ֽ��ϴ�.");
            picoCoinValue.text = "<color=yellow> ��ġ�� ������ ���� �����ϴ�. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedPicoCoin - ExchangeManager.Instance.originCoinExchangeRate) <= 0.5f) && _tempCalculatedPicoCoin - ExchangeManager.Instance.originCoinExchangeRate > 0)
        {
            Debug.Log($"���� ������ ���� {_tempCalculatedPicoCoin}�̹Ƿ� ���� ��� �� �Դϴ�.");
            picoCoinValue.text = "<color=#fc4c4e> ��ġ ��� �� �Դϴ�. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedPicoCoin - ExchangeManager.Instance.originCoinExchangeRate) <= 0.5f) && _tempCalculatedPicoCoin - ExchangeManager.Instance.originCoinExchangeRate < 0)
        {
            Debug.Log($"���� ������ ���� {_tempCalculatedPicoCoin}�̹Ƿ� ���� �϶� �� �Դϴ�.");
            picoCoinValue.text = "<color=#0c43b7> ��ġ �϶� �� �Դϴ�. </color>";
        }
        else
        {
            picoCoinValue.text = "<color=grey> ��ġ �Ǵ� �� �Դϴ�. </color>";
        }

        #endregion

        #region �۸��� ũ����Ż

        float _tempGlitterFloat = 0;

        foreach (float GlitterCrystalExchangeRate in ExchangeManager.Instance.glitterCrystalTrends)
        {
            _tempGlitterFloat += GlitterCrystalExchangeRate;
        }

        float _tempCalculatedGlitterCrystal = _tempGlitterFloat / ExchangeManager.Instance.glitterCrystalTrends.Count;

        if (Mathf.Abs(_tempCalculatedGlitterCrystal - ExchangeManager.Instance.originCrystalExchangeRate) <= 0.5f)
        {
            Debug.Log($"�۸��� ũ����Ż�� ���� {_tempCalculatedGlitterCrystal}�̹Ƿ� +- 0.5����Ʈ ���� ���� �ֽ��ϴ�.");
            glitterCrystalValue.text = "<color=yellow> ��ġ�� ������ ���� �����ϴ�. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedGlitterCrystal - ExchangeManager.Instance.originCrystalExchangeRate) <= 0.5f) && _tempCalculatedGlitterCrystal - ExchangeManager.Instance.originCrystalExchangeRate > 0)
        {
            Debug.Log($"�۸��� ũ����Ż�� ���� {_tempCalculatedGlitterCrystal}�̹Ƿ� ���� ��� �� �Դϴ�.");
            glitterCrystalValue.text = "<color=#fc4c4e> ��ġ ��� �� �Դϴ�. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedGlitterCrystal - ExchangeManager.Instance.originCrystalExchangeRate) <= 0.5f) && _tempCalculatedGlitterCrystal - ExchangeManager.Instance.originCrystalExchangeRate < 0)
        {
            Debug.Log($"�۸��� ũ����Ż�� ���� {_tempCalculatedGlitterCrystal}�̹Ƿ� ���� �϶� �� �Դϴ�.");
            glitterCrystalValue.text = "<color=#0c43b7> ��ġ �϶� �� �Դϴ�. </color>";
        }
        else
        {
            glitterCrystalValue.text = "<color=grey> ��ġ �Ǵ� �� �Դϴ�. </color>";
        }

        #endregion

        #region �긯 ����

        float _tempBrickCoinFloat = 0;

        foreach (float brickCoinExchangeRate in ExchangeManager.Instance.brickCoinTrends)
        {
            _tempBrickCoinFloat += brickCoinExchangeRate;
        }

        float _tempCalculatedbrickCoin = _tempBrickCoinFloat / ExchangeManager.Instance.brickCoinTrends.Count;

        if (Mathf.Abs(_tempCalculatedbrickCoin - ExchangeManager.Instance.originBrickExchangeRate) <= 0.5f)
        {
            Debug.Log($"�긯 ������ ���� {_tempCalculatedbrickCoin}�̹Ƿ� +- 0.5����Ʈ ���� ���� �ֽ��ϴ�.");
            brickCoinValue.text = "<color=yellow> ��ġ�� ������ ���� �����ϴ�. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedbrickCoin - ExchangeManager.Instance.originBrickExchangeRate) <= 0.5f) && _tempCalculatedbrickCoin - ExchangeManager.Instance.originBrickExchangeRate > 0)
        {
            Debug.Log($"�긯 ������ ���� {_tempCalculatedbrickCoin}�̹Ƿ� ���� ��� �� �Դϴ�.");
            brickCoinValue.text = "<color=#fc4c4e> ��ġ ��� �� �Դϴ�. </color>";
        }
        else if (!(Mathf.Abs(_tempCalculatedbrickCoin - ExchangeManager.Instance.originBrickExchangeRate) <= 0.5f) && _tempCalculatedbrickCoin - ExchangeManager.Instance.originBrickExchangeRate < 0)
        {
            Debug.Log($"�긯 ������ ���� {_tempCalculatedbrickCoin}�̹Ƿ� ���� �϶� �� �Դϴ�.");
            brickCoinValue.text = "<color=#0c43b7> ��ġ �϶� �� �Դϴ�. </color>";
        }
        else
        {
            brickCoinValue.text = "<color=grey> ��ġ �Ǵ� �� �Դϴ�. </color>";
        }

        Debug.Log("��� �Ϸ�");
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

    }   // inputResource���� ȭ�� ��ư Ŭ�� �� �ߵ��ϴ� �ż���

    private void SetupQuantityDesignationPanel()
    {
        refreshTable_Text.text = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}�� ���ΰ�ħ";

        switch (seletedInputCurrency)
        {
            case 0:
                inputCurrency_Text.text = "���� ����(PCC)";
                inputExchangeRateCurrency_Text.text = ExchangeManager.Instance.coinExchangeRate.ToString("F4");
                myCurrencyName_Text.text = "���� ���� ����(PCC)";
                myCurrencyPoint_Text.text = PlayerWalletManager.Instance.TotalCoin.ToString("#,##0");
                inputFeeRateCurrency_Text.text = ExchangeManager.Instance.coinFeeRate.ToString("F2");
                break;
            case 1:
                inputCurrency_Text.text = "�۸��� ũ����Ż(GLTC)";
                inputExchangeRateCurrency_Text.text = ExchangeManager.Instance.crystalExchangeRate.ToString("F4");
                myCurrencyName_Text.text = "���� �۸��� ũ����Ż(GLTC)";
                myCurrencyPoint_Text.text = PlayerWalletManager.Instance.TotalCrystal.ToString("#,##0");
                inputFeeRateCurrency_Text.text = ExchangeManager.Instance.crystalFeeRate.ToString("F2");
                break;
            case 2:
                inputCurrency_Text.text = "�긯 ����(BC)";
                inputExchangeRateCurrency_Text.text = ExchangeManager.Instance.brickExchangeRate.ToString("F4");
                myCurrencyName_Text.text = "���� �긯 ����(BC)";
                myCurrencyPoint_Text.text = PlayerWalletManager.Instance.TotalBrick.ToString("#,##0");
                inputFeeRateCurrency_Text.text = ExchangeManager.Instance.brickFeeRate.ToString("F2");
                break;
        }

        switch (seletedOutputCurrency)
        {
            case 0:
                outputCurrency_Text.text = "���� ����(PCC)";
                outputExchangeRateCurrency_Text.text = ExchangeManager.Instance.coinExchangeRate.ToString("F4");
                exchangeCurrencyName_Text.text = "��ȯ ���� ����(PCC)";
                outputFeeRateCurrency_Text.text = ExchangeManager.Instance.coinFeeRate.ToString("F2");
                break;
            case 1:
                outputCurrency_Text.text = "�۸��� ũ����Ż(GLTC)";
                outputExchangeRateCurrency_Text.text = ExchangeManager.Instance.crystalExchangeRate.ToString("F4");
                exchangeCurrencyName_Text.text = "��ȯ �۸��� ũ����Ż(GLTC)";
                outputFeeRateCurrency_Text.text = ExchangeManager.Instance.crystalFeeRate.ToString("F2");
                break;
            case 2:
                outputCurrency_Text.text = "�긯 ����(BC)";
                outputExchangeRateCurrency_Text.text = ExchangeManager.Instance.brickExchangeRate.ToString("F4");
                exchangeCurrencyName_Text.text = "��ȯ �긯 ����(BC)";
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

        float inputFeeAmount = 0;   // �Է� ȭ���� ������ ��� ����
        float inputFinalFee = 0;    // �Է� ȭ���� ������ ��� �� �� ���� ����

        float finalOutputAmount = 0;    // ��� ȭ���� �� ��� ����

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

        checkExchange_Text.text = $"{inputCurrency_Text.text}(��)�� {outputCurrency_Text.text}(��)�� {Mathf.RoundToInt(finalOutputAmount)}��ŭ ��ȯ�Ѵ�.";
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
                        Debug.LogError($"�÷��̾� ȭ��: {PlayerWalletManager.Instance.TotalCoin} �Է� ��: {exchangeAmount}");
                        return;
                    }
                }
                break;
            case 1:
                if (int.TryParse(exchangeCurrencyAmount_InputField.text, out exchangeAmount))
                {
                    if (PlayerWalletManager.Instance.TotalCrystal < exchangeAmount)
                    {
                        Debug.LogError($"�÷��̾� ȭ��: {PlayerWalletManager.Instance.TotalCoin} �Է� ��: {exchangeAmount}");
                        return;
                    }
                }
                break;
            case 2:
                if (int.TryParse(exchangeCurrencyAmount_InputField.text, out exchangeAmount))
                {
                    if (PlayerWalletManager.Instance.TotalBrick < exchangeAmount)
                    {
                        Debug.LogError($"�÷��̾� ȭ��: {PlayerWalletManager.Instance.TotalCoin} �Է� ��: {exchangeAmount}");
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
        float inputFeeAmount = 0;   // �Է� ȭ���� ������ ��� ����
        float inputFinalFee = 0;    // �Է� ȭ���� ������ ��� �� �� ���� ����

        float finalOutputAmount = 0;    // ��� ȭ���� �� ��� ����

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
