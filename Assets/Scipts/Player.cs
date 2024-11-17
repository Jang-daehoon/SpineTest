using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트 에서는 애니메이션이 변경되는 동작에 해당하는 변경이 이루어진다.
//Idle->Walk 같은 큰 동작이 변경된다.
namespace SpineTest
{
    public class Player : MonoBehaviour
    {
        #region public Field
        //스파인 애니메이션을 위한 것
        public SkeletonAnimation skeletonAnimation;
        public AnimationReferenceAsset[] animClip;

        //애니메이션에 대한 ENUM
        public enum AnimState
        {
            IDLE, BLINK
        }
        #endregion
        #region private Field
        //현재 애니메이션 처리가 무엇인지에 대한 변수
        private AnimState animState;
        private string currentAnimation;    //현재 어떤 애니메이션이 재생되고 있는지에 대한 변수

        //Move처리
        private Rigidbody2D rig;
        private float xx;
        #endregion

        private void Awake() => rig = GetComponent<Rigidbody2D>();

        private void Update()
        {
            xx = Input.GetAxisRaw("Horizontal");

            if (xx == 0f)
                animState = AnimState.IDLE;
            else
            {
                animState = AnimState.BLINK;
                transform.localScale = new Vector2(xx, 1);
            }

            //애니메이션 적용
            SetCurrentAnimation(animState);

        }
        private void FixedUpdate()=> rig.velocity = new Vector2(xx * 300 * Time.deltaTime, rig.velocity.y);

        private void _AsncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)
        {
            if (animClip.name.Equals(currentAnimation)) return; //매 프레임마다 재실행되지 않도록 리턴.

            //해당 애니메이션으로 변경한다.
            skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
            skeletonAnimation.loop = loop;
            skeletonAnimation.timeScale = timeScale;

            //현재 재생되고 있는 애니메이션 값을 변경.
            currentAnimation = animClip.name;
        }

        private void SetCurrentAnimation(AnimState _state)
        {
            switch(_state)
            {
                case AnimState.IDLE:
                    _AsncAnimation(animClip[(int)AnimState.IDLE], true, 1f);
                    break;
                case AnimState.BLINK:
                    _AsncAnimation(animClip[(int)AnimState.BLINK], true, 1f);
                    break;
            }
            //짧게 개선한 코드
            //_AsncAnimation(animClip[(int)_state],true, 1f);  

        }

    }
}
