using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCombiner
{
    //본이름을 string으로 저장하지 않고 Hash Code로 int형으로 변환하여 저장. 스트링 비교연산 부하를 줄인다.
    private readonly Dictionary<int, Transform> _rootBoneDictionary = new Dictionary<int, Transform>();
    //root의 transform
    private readonly Transform transform;

    public EquipmentCombiner(GameObject rootGO)
    {
        transform = rootGO.transform;
        TraverseHierachy(transform);
    }

    /// <summary>
    /// 뼈대에대한 정보 저장
    /// </summary>
    /// <param name="root"></param>
    public void TraverseHierachy(Transform root)
    {
        foreach (Transform child in root)
        {
            _rootBoneDictionary.Add(child.name.GetHashCode(), child);

            //리커시브 자료구조 기법으로 순환저장
            TraverseHierachy(child);
        }
    }

    /// <summary>
    /// skinned mesh아이템을 장착, 본을 심은 후 자식으로 단다.
    /// </summary>
    /// <param name="boneGO">장착할 아이템의 model</param>
    /// <param name="boneNames">아이템의 boneNames</param>
    /// <returns></returns>
    public Transform AddLimb(GameObject itemGO, List<string> boneNames)
    {
        Transform limb = ProcessBoneObject(itemGO.GetComponentInChildren<SkinnedMeshRenderer>(), boneNames);
        limb.SetParent(transform);

        return limb;
    }

    /// <summary>
    /// 기존 스킨드메쉬 모델을 새로운 게임오브젝트를 생성하여 현재 아이템을 장착시킬 bone들을 심는다.
    /// </summary>
    /// <param name="modelRenderer">장착할 아이템 model</param>
    /// <param name="boneNames">아이템의 boneNames</param>
    /// <returns></returns>
    private  Transform ProcessBoneObject(SkinnedMeshRenderer modelRenderer, List<string> boneNames)
    {
        //새 게임오브젝트를 생성
        Transform newItemTransform = new GameObject().transform;
        SkinnedMeshRenderer newMeshRenderer = newItemTransform.gameObject.AddComponent<SkinnedMeshRenderer>();

        //기존 본의 Transform을 가져온다
        Transform[] boneTransforms = new Transform[boneNames.Count];
        for (int i = 0; i < boneNames.Count; ++i)
        {
            boneTransforms[i] = _rootBoneDictionary[boneNames[i].GetHashCode()];
        }

        //새로 생성된 아이템으로 복제
        newMeshRenderer.bones = boneTransforms;
        newMeshRenderer.sharedMesh = modelRenderer.sharedMesh;
        newMeshRenderer.materials = modelRenderer.sharedMaterials;

        return newItemTransform;
    }

    /// <summary>
    /// static Mesh 아이템 장착(무기등)
    /// </summary>
    /// <param name="itemGO"></param>
    /// <returns>견갑같은경우는 2개이므로 2개의 transform반환이 있을 수 있다. (스킨드메쉬는 분리되어있어도 하나로 인식하지만, static mesh는 각각 독립적으로 구성된다.)</returns>
    public Transform[] AddMesh(GameObject itemGO)
    {
        Transform[] itemTransforms = ProcessBoneObject(itemGO.GetComponentsInChildren<MeshRenderer>());
        return itemTransforms;
    }

    private Transform[] ProcessBoneObject(MeshRenderer[] meshRenderers)
    {
        List<Transform> newItemTransforms = new List<Transform>();

        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (renderer.transform.parent != null)
            {
                Transform parent = _rootBoneDictionary[renderer.transform.parent.name.GetHashCode()]; //숄더같은경우 어깨본을 찾아야하기떄문에

                GameObject itemGO = GameObject.Instantiate(renderer.gameObject, parent);
                newItemTransforms.Add(itemGO.transform);
            }
        }

        return newItemTransforms.ToArray();
    }
}
