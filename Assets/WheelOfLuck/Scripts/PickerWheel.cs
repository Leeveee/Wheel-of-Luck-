using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WheelOfLuck.Scripts.Config;
using Random = UnityEngine.Random;

namespace WheelOfLuck.Scripts
{
    public class PickerWheel : MonoBehaviour
    {
        public event Action OnSpinStart;
        public event Action<int> OnSpinEnd;
        
        [Header("References :")]
        [SerializeField]
        private GameObject _linePrefab;
        [SerializeField]
        private Transform _linesParent;

        [Space]
        [SerializeField]
        private Transform _wheelCircle;
        [SerializeField]
        private PieceView _wheelPiecePrefab;
        [SerializeField]
        private Transform _wheelPiecesParent;

        [Space]
        [Header("Sounds :")]
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _clip;
        [SerializeField]
        [Range(0f, 1f)]
        private float _volume = 0.3f;
        [SerializeField]
        [Range(-3f, 3f)]
        private float _pitch = 1f;

        [Space]
        [Header("Wheel of Luck  settings :")]
        [Range(1, 20)]
        public int _spinDuration = 3;
        [SerializeField]
        private int _countTurn;

        [Space]
        [SerializeField]
        private WheelPiecesSetting _wheelPieces;

        private readonly WheelPieceService _wheelPieceService = new WheelPieceService();
        private readonly List<PieceView> _pieceViewList = new List<PieceView>();

        private bool _isSpinning;
        private float pieceAngle;
        private float halfPieceAngle;
        private float halfPieceAngleWithPaddings;

        private void Awake()
        {
            OnSpinEnd += RegeneratePiece;
        }

        private void Start()
        {
            InitializePieceAngles();
            Generate();
            SetupAudio();
            _wheelPieceService.CalculateWeightsAndIndices();
        }

        private void OnDestroy()
        {
            OnSpinEnd -= RegeneratePiece;
            _wheelPieceService.WheelPieceList.Clear();
        }

        private void InitializePieceAngles()
        {
            pieceAngle = 360 / _wheelPieces.Pieces.Count;
            halfPieceAngle = pieceAngle / 2f;
            halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f);
        }

        private void RegeneratePiece (int index)
        {
            WheelPiece piece = _wheelPieceService.GetSupportPiece(_wheelPieces);

            if (piece== null)
                return;
            
            _pieceViewList[index].SetPieceView(piece.Icon, piece.Label, piece.Amount.ToString());

            _wheelPieceService.WheelPieceList[index] = piece;
            _wheelPieceService.CalculateWeightsAndIndices();
        }

        private void SetupAudio()
        {
            _audioSource.clip = _clip;
            _audioSource.volume = _volume;
            _audioSource.pitch = _pitch;
        }

        private void Generate()
        {
            _wheelPieceService.WheelPieceList.Clear();

            for (int i = 0; i < _wheelPieces.Pieces.Count; i++)
                DrawPiece(i);
        }

        private void DrawPiece (int index)
        {
            WheelPiece piece = _wheelPieces.Pieces[index];
            GameObject gameObjectPiece = InstantiatePiece();
            PieceView pieceView = gameObjectPiece.GetComponent<PieceView>();
            pieceView.SetPieceView(piece.Icon, piece.Label, piece.Amount.ToString());

            Transform line = Instantiate(_linePrefab, _linesParent.position, Quaternion.identity, _linesParent).transform;
            line.RotateAround(_wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle);

            gameObjectPiece.transform.RotateAround(_wheelPiecesParent.position, Vector3.back, pieceAngle * index);
            _wheelPieceService.WheelPieceList.Add(_wheelPieces.Pieces[index]);
            _pieceViewList.Add(pieceView);
        }

        private GameObject InstantiatePiece()
        {
            return Instantiate(_wheelPiecePrefab.gameObject, _wheelPiecesParent.position, Quaternion.identity, _wheelPiecesParent);
        }
        
        public void Spin()
        {
            if (!_isSpinning)
            {
                _isSpinning = true;
                OnSpinStart?.Invoke();

                int index = _wheelPieceService.GetRandomPieceIndex();

                float angle = -(pieceAngle * index);
                float rightOffset = (angle - halfPieceAngleWithPaddings) % 360;
                float leftOffset = (angle + halfPieceAngleWithPaddings) % 360;
                float randomAngle = Random.Range(leftOffset, rightOffset);
                Vector3 targetRotation = Vector3.back * (randomAngle + _countTurn * 360);
                float currentAngle;
                float prevAngle = currentAngle = _wheelCircle.eulerAngles.z;
                bool isIndicatorOnTheLine = false;

                _wheelCircle.DORotate(targetRotation, _spinDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.InOutQuart)
                    .OnUpdate(() =>
                    {
                        float diff = Mathf.Abs(prevAngle - currentAngle);

                        if (diff >= halfPieceAngle)
                        {
                            if (isIndicatorOnTheLine)
                            {
                                _audioSource.PlayOneShot(_audioSource.clip);
                            }

                            prevAngle = currentAngle;
                            isIndicatorOnTheLine = !isIndicatorOnTheLine;
                        }

                        currentAngle = _wheelCircle.eulerAngles.z;
                    })
                    .OnComplete(() =>
                    {
                        _isSpinning = false;
                        Debug.Log($"Collected {_wheelPieceService.WheelPieceList[index].Label} {_wheelPieceService.WheelPieceList[index].Amount}" );
                        OnSpinEnd?.Invoke(index);

                    });

            }
        }
    }
}