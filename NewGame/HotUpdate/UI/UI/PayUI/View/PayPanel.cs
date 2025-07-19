using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
namespace HotUpdate
{

    public partial class PayPanel : PanelBase
    {
        private BarcodeWriter mWriter;


        private Texture2D encoded;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            SetUpPanel();
           StartCoroutine(TimeUtil.TimeCountDown(900, m_TxtM_Tempo));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }

        protected override void Update()
        {
            base.Update();
            //m_TxtM_Tempo.text = 
            //if (true)
            //{
            //    var timeDown = ConfigCtrl.Instance.Tables.TbConst_Config.DataList.Find(x=>x.Id==8).Val1;
            //}
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_connect.onClick.AddListener(OnCopyBtn);
            m_Btn_Buy.onClick.AddListener(OnCloseBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_connect.onClick.RemoveListener(OnCopyBtn);
            m_Btn_Buy.onClick.RemoveListener(OnCloseBtn);
        }
        #endregion

        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("PayPanel");
        }
        public void OnCopyBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            GUIUtility.systemCopyBuffer = m_TxtM_Pagar.text;
            ToolUtil.FloattingText("Copiar com sucesso,por favor abra PIX", this.gameObject.transform);
        }
        public void SetUpPanel()
        {
            m_TxtM_date.text = $"{TimeUtil.TimestampToDataTime(MainUIModel.Instance.orderData.CreatOderTime)}";
            m_TxtM_Numero.text = $"{MainUIModel.Instance.orderData.OrderId}";
            m_TxtM_Pagar.text = $"{MainUIModel.Instance.orderData.QrcodeRaw}";
            CreatQR(m_RImg_QRCode, MainUIModel.Instance.orderData.QrcodeRaw, 325, 325);
        }
        public static Color32[] Encode(string textForEncoding, int width, int height)
        {
            QrCodeEncodingOptions options = new QrCodeEncodingOptions();
            options.CharacterSet = "UTF-8";
            options.Width = width;
            options.Height = height;
            options.Margin = 1;
            BarcodeWriter barcodeWriter = new BarcodeWriter
            { Format = BarcodeFormat.QR_CODE, Options = options };
            return barcodeWriter.Write(textForEncoding);
        }
        /// <summary>  
        /// 生成二维码  
        /// </summary>  
        public static Image CreatQR(Image img, string qrcode, int width, int height)
        {
            Texture2D encoded = null;
            if (width == height && width == 256)
            {
                encoded = GenerateQRCode256(qrcode, width, height);
            }
            else
            {
                encoded = GenerateQRCode(qrcode, width, height);
            }
            Sprite sprite = Sprite.Create(encoded, new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f));
            img.sprite = sprite;
            return img;
        }
        public static RawImage CreatQR(RawImage img, string qrcode, int width, int height)
        {
            Texture2D encoded = null;
            if (width == height && width == 256)
            {
                encoded = GenerateQRCode256(qrcode, width, height);
            }
            else
            {
                encoded = GenerateQRCode(qrcode, width, height);
            }
            img.texture = encoded;
            return img;
        }

        /// <summary>
        /// 生成256的二维码
        /// </summary>
        public static Texture2D GenerateQRCode256(string str, int width, int height)
        {
            Texture2D t = new Texture2D(width, height);
            Color32[] col32 = Encode(str, width, height);
            t.SetPixels32(col32);
            t.Apply();
            return t;
        }

        /// <summary>
        /// 生成2维码 方法
        /// 经测试：能生成任意尺寸的正方形
        /// </summary>
        public static Texture2D GenerateQRCode(string qrcodeText, int width, int height)
        {
            // 编码成color32
            MultiFormatWriter writer = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
            hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            hints.Add(EncodeHintType.MARGIN, 1);
            hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.M);
            BitMatrix bitMatrix = writer.encode(qrcodeText, BarcodeFormat.QR_CODE, width, height, hints);
            int w = bitMatrix.Width;
            int h = bitMatrix.Height;
            Texture2D texture = new Texture2D(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //向右翻转90度
                    if (bitMatrix[height - 1 - y, x])
                    {
                        texture.SetPixel(x, y, Color.black);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                }
            }
            texture.Apply();
            return texture;
        }





    }
}
