using System.Collections.Generic;
using UnityEngine;

namespace WheelOfLuck.Scripts.Config
{
    [CreateAssetMenu(fileName = "WheelPiecesSetting", menuName = "Config/Splin/WheelPiecesSetting")]
    public class  WheelPiecesSetting : ScriptableObject
    {
        [SerializeField]
        private List<WheelPiece> _pieces;
        [SerializeField]
        private bool _useRandomSupportPiecesList;
        [SerializeField]
        private List<WheelPiece> _additionalPieces;

        public List<WheelPiece> Pieces =>_pieces;
        public List<WheelPiece> AdditionalPieces => _additionalPieces;
        public bool UseRandomSupportPiecesList => _useRandomSupportPiecesList;
    }
}