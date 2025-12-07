using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Scripts.Extension
{
    public class TextAmount : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtAmount;

        public void Init(string prefix, int amount, Vector2 txtAmountPos)
        {
            TxtAmount(prefix, amount, txtAmountPos);
        }
        
        public void TxtAmount(string prefix, int amount, Vector2 txtAmountPos)
        {
            txtAmount.text = $"{prefix}{amount}";
            txtAmount.transform.localScale = Vector3.zero;
            txtAmount.transform.localPosition = txtAmountPos;
            txtAmount.transform.DOLocalMoveY(txtAmountPos.y + 100f, 1.5f).SetEase(Ease.Linear);
            txtAmount.transform.DOScale(1, 0.38f).SetEase(Ease.Linear);
            txtAmount.DOFade(1, 0.38f).SetEase(Ease.OutSine);
            txtAmount.DOFade(0, 0.75f).SetEase(Ease.InCubic).SetDelay(0.75f);
        }
    }
}