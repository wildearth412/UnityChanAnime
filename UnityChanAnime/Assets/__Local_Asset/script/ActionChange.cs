using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace ActionChange
{
    using ObjectChange;
    [RequireComponent(typeof(Animator))]
    
    public class ActionChange : MonoBehaviour
	{
		private Animator anim;						// Animatorへの参照
		private AnimatorStateInfo currentState;		// 現在のステート状態を保存する参照
		private AnimatorStateInfo previousState;	// ひとつ前のステート状態を保存する参照
        [SerializeField]
        private ObjectChange objectChanged = null;
        [SerializeField]
        private AudioSource onsei = null;
        [SerializeField]
        private List<AudioClip> onseiClips = new List<AudioClip>();
        [SerializeField]
        [Tooltip("シーンの番号　→　0,1")]
        private int currentScene = 0;               // main scene index　　　　0 , 1 
        private bool isCanBeChanged = false;        //シーン変換の許可があるかどうか
        [SerializeField]
        private bool loopSwitch = false;	        // loop判定スタートスイッチ
        [SerializeField]
        private float loopInterval = 5.0f;         // loop判定のインターバル
        private bool loopFlag = false;              // loop判定flag


        void Start ()
		{
			// 各参照の初期化
			anim = GetComponent<Animator> ();
			currentState = anim.GetCurrentAnimatorStateInfo (0);
			previousState = currentState;
			// loop判定用関数をスタートする
			StartCoroutine ("LoopChange");
		}
	
		void  Update ()
		{
            if (isCanBeChanged)
            {
                loopSwitch = false;
                // メッセージが届いたら、scene1に送る処理
                if (currentScene == 1)
                {
                    onsei.clip = onseiClips[1];
                    anim.SetBool("Next", true);
                    objectChanged.SetStatus(0, false);
                    objectChanged.SetStatus(1, false);
                    objectChanged.SetStatus(2, true);
                }

                // メッセージが届いたら、scene0に戻す処理
                if (currentScene == 0)
                {
                    onsei.clip = onseiClips[0];
                    anim.SetBool("Back", true);
                    objectChanged.SetStatus(0, true); 
                    objectChanged.SetStatus(2, false);
                }
            }
		
			// "Next"フラグがtrueの時の処理
			if (anim.GetBool ("Next")) {
				// 現在のステートをチェックし、ステート名が違っていたらブーリアンをfalseに戻す、つまり動作の変換を停止
				currentState = anim.GetCurrentAnimatorStateInfo (0);
				if (previousState.fullPathHash != currentState.fullPathHash) {
					anim.SetBool ("Next", false);
					previousState = currentState;
				}
			}
		
			// "Back"フラグがtrueの時の処理
			if (anim.GetBool ("Back")) {
                // 現在のステートをチェックし、ステート名が違っていたらブーリアンをfalseに戻す、つまり動作の変換を停止
                currentState = anim.GetCurrentAnimatorStateInfo (0);
				if (previousState.fullPathHash != currentState.fullPathHash) {
					anim.SetBool ("Back", false);
					previousState = currentState;
                    loopSwitch = true;
                }
			}

            if (anim.GetBool("LoopNext"))
            {
                currentState = anim.GetCurrentAnimatorStateInfo(0);
                if (previousState.fullPathHash != currentState.fullPathHash)
                {
                    anim.SetBool("LoopNext", false);
                    previousState = currentState;
                }
            }
            else if (anim.GetBool("LoopBack"))
            {
                currentState = anim.GetCurrentAnimatorStateInfo(0);
                if (previousState.fullPathHash != currentState.fullPathHash)
                {
                    anim.SetBool("LoopBack", false);
                    previousState = currentState;
                }
            }

            //メッセージをリセット
            if(isCanBeChanged)
            {
                isCanBeChanged = false;
            }
		}

		void OnGUI ()
		{
			GUI.Box (new Rect (Screen.width - 110, 10, 100, 90), "Change Scene");
            if (GUI.Button(new Rect(Screen.width - 100, 40, 80, 20), "Next"))
            {
                if (currentScene == 0)
                {
                    anim.SetBool("LoopNext", false);
                    anim.SetBool("LoopBack", false);
                    currentScene = 1;
                    isCanBeChanged = true;
                }
            }
            if (GUI.Button(new Rect(Screen.width - 100, 70, 80, 20), "Back"))
            {
                if (currentScene == 1)
                {
                    anim.SetBool("LoopNext", false);
                    anim.SetBool("LoopBack", false);
                    currentScene = 0;
                    isCanBeChanged = true;
                }
            }
		}

        public void Next()
        {
            if (currentScene == 0)
            {
                anim.SetBool("LoopNext", false);
                anim.SetBool("LoopBack", false);
                currentScene = 1;
                isCanBeChanged = true;
            }
        }

        public void Back()
        {
            if (currentScene == 1)
            {
                anim.SetBool("LoopNext", false);
                anim.SetBool("LoopBack", false);
                currentScene = 0;
                isCanBeChanged = true;
            }
        }

        // loop判定用関数
        IEnumerator LoopChange ()
		{
			// 無限ループ開始
			while (true) {
				//loop判定スイッチオンの場合
				if (loopSwitch) {
                    if (currentScene == 0)
                    {
                        // loopフラグ設定をする
                        loopFlag = !loopFlag;
                        if (!loopFlag)
                        {
                            anim.SetBool("LoopBack", true);
                            anim.SetBool("LoopNext", false);
                            objectChanged.SetStatus(0, true);
                            objectChanged.SetStatus(1, false);
                        }
                        else
                        {
                            anim.SetBool("LoopNext", true);
                            anim.SetBool("LoopBack", false);
                            objectChanged.SetStatus(0, false);
                            objectChanged.SetStatus(1, true);
                        }
                    }
				}
				// 次の判定までインターバルを置く
				yield return new WaitForSeconds (loopInterval);
			}
		}
	}
}
