using System.Collections.Generic;
using UnityEngine;
using WheelOfLuck.Scripts.Config;

namespace WheelOfLuck.Scripts
{
    public class WheelPieceService 
    {
        public readonly List<WheelPiece> WheelPieceList = new List<WheelPiece>();
        private double _totalWeight;
        private int _lastReturnedBasePieceIndex = 1;
        private int _lastReturnedSupportPieceIndex = 1;
        private readonly System.Random _random = new System.Random();

        public int GetRandomPieceIndex()
        {
            int index = WheelPieceList.FindIndex(piece => piece.SequenceNumber == _lastReturnedBasePieceIndex);

            if (index != -1)
            {
                _lastReturnedBasePieceIndex++;
                return index;
            }

            double random = _random.NextDouble() * _totalWeight;

            for (int i = 0; i < WheelPieceList.Count; i++)
                if (WheelPieceList[i]._weight >= random)
                    return WheelPieceList[i].Index;

            return 0;
        }
        
        public void CalculateWeightsAndIndices()
        {
            _totalWeight = 0;

            for (int i = 0; i < WheelPieceList.Count; i++)
            {
                WheelPiece piece = WheelPieceList[i];

                _totalWeight += piece.Chance;
                piece._weight = _totalWeight;

                piece.Index = i;
            }
        }
        
        public WheelPiece GetSupportPiece (WheelPiecesSetting wheelPiecesSetting)
        {
            if (wheelPiecesSetting.AdditionalPieces.Count == 0)
                return null;
            
            if (wheelPiecesSetting.UseRandomSupportPiecesList)
            {
                return wheelPiecesSetting.AdditionalPieces[Random.Range(0, wheelPiecesSetting.AdditionalPieces.Count)];
            }

            int index = wheelPiecesSetting.AdditionalPieces.FindIndex(piece => piece.SequenceNumber == _lastReturnedSupportPieceIndex);

            if (index != -1)
            {
                _lastReturnedSupportPieceIndex++;
                return wheelPiecesSetting.AdditionalPieces[index];
            }

            return wheelPiecesSetting.AdditionalPieces[Random.Range(0, wheelPiecesSetting.AdditionalPieces.Count)];
        }
    }
}