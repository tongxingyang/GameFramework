using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.UIExtension.UIEmojiText
{
    public class UIEmojiTextGraphicBoard : MaskableGraphic
    {
        private readonly VertexHelper s_VertexHelper = new VertexHelper();
        private IList<UIEmotionTextModel> _Models;

        public void SetVertexData(List<UIEmotionTextModel> models)
        {
            _Models = models;
            DoMeshGeneration();
        }
        private void DoMeshGeneration()
        {
            if (rectTransform != null && rectTransform.rect.width >= 0 && rectTransform.rect.height >= 0)
                OnPopulateMesh(s_VertexHelper);
            else
                s_VertexHelper.Clear();
            s_VertexHelper.FillMesh(workerMesh);
            canvasRenderer.SetMesh(workerMesh);
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
            if (_Models == null || _Models.Count <= 0) return;
            foreach (UIEmotionTextModel model in _Models)
            {
                AddQuad(toFill, model);
            }
        }
        
        void AddQuad(VertexHelper vertexHelper, UIEmotionTextModel model)
        {
            int startIndex = vertexHelper.currentVertCount;
        
            vertexHelper.AddVert(new Vector3(model.PosMin.x, model.PosMin.y, 0), color, new Vector2(model.X, model.Y + model.Size));
            vertexHelper.AddVert(new Vector3(model.PosMax.x, model.PosMin.y, 0), color, new Vector2(model.X + model.Size, model.Y + model.Size));
            vertexHelper.AddVert(new Vector3(model.PosMax.x, model.PosMax.y, 0), color, new Vector2(model.X + model.Size, model.Y));
            vertexHelper.AddVert(new Vector3(model.PosMin.x, model.PosMax.y, 0), color, new Vector2(model.X, model.Y));
        
            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }
    }
}