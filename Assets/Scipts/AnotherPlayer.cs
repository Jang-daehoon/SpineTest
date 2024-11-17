using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpineTest
{
    public class AnotherPlayer : MonoBehaviour
    {
        #region public Field
        //스파인 애니메이션을 위한 것
        public SkeletonAnimation skeletonAnimation;
        public AnimationReferenceAsset[] animClip;

        //애니메이션에 대한 ENUM
        public enum AnimState
        {
            IDLE, WALK, RUN, ATTACK, JUMP, MORNINGSTARPOS
        }
        #endregion
        #region private Field
        //현재 애니메이션 처리가 무엇인지에 대한 변수
        private AnimState animState;
        private string currentAnimation;    //현재 어떤 애니메이션이 재생되고 있는지에 대한 변수

        //Move처리
        private Rigidbody2D rig;
        private float xx;
        private float minimumDelay = 0.15f;
        private float maximumDelay = 3f;

        private bool isAttackMode;
        #endregion

        private void Awake()
        {
            rig = GetComponent<Rigidbody2D>();
            isAttackMode = false;

        }
        private void Start()
        {
            animState = AnimState.IDLE;
        }
        private void Update()
        {
            xx = Input.GetAxisRaw("Horizontal");

            if (xx == 0f)
                animState = AnimState.IDLE;
            else
            {
                animState = AnimState.WALK;
                transform.localScale = new Vector2(xx, 1);
            }
            //공격시
            if(Input.GetKeyDown(KeyCode.Space))
            {
                //스킨 설정 (예: "weapon/sword")
                skeletonAnimation.initialSkinName = "weapon/sword";
                skeletonAnimation.Initialize(true); // 스킨 초기화를 적용
                //ChangeFaceOnly(animClip[(int)AnimState.ATTACK], false, 1f, 1);

                //1회 공격 후 원래 애니메이션을 복귀
                ChangeFaceOne(animClip[(int)AnimState.ATTACK], false, 1f, animClip[(int)AnimState.IDLE]);
            }

            //STATE에 따른 애니메이션 적용 애니메이션은 모두 비동기 처리
            SetCurrentAnimation(animState);

        }
        private void FixedUpdate()
        {
            rig.velocity = new Vector2 (xx * 300 * Time.deltaTime, rig.velocity.y);
        }

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
        private void ChangeFaceOnly(AnimationReferenceAsset faceAnimClip, bool loop, float timeScale, int track = 1)
        {
            // 지정된 트랙(1번 트랙)에서 애니메이션 재생
            TrackEntry entry = skeletonAnimation.state.SetAnimation(track, faceAnimClip, loop);
            entry.TimeScale = timeScale;
          
            // 표정 애니메이션이 종료되면 해당 트랙을 비우거나 유지
            if (!loop)
            {
                entry.Complete += (trackEntry) =>
                {
                    skeletonAnimation.state.ClearTrack(track); // 표정 애니메이션이 끝나면 트랙을 비움
                };
            }
        }
        private void ChangeFaceOne(AnimationReferenceAsset animClip, bool loop, float timeScale, AnimationReferenceAsset defaultAnimClip, int track = 0)
        {
            // 지정된 애니메이션 재생
            TrackEntry entry = skeletonAnimation.state.SetAnimation(track, animClip, loop);
            entry.TimeScale = timeScale;

            // 애니메이션 종료 시 기본 상태로 복원
            entry.Complete += (trackEntry) =>
            {
                skeletonAnimation.state.SetAnimation(track, defaultAnimClip, true).TimeScale = 1f; // 기본 애니메이션으로 복원
            };
        }

        private void SetCurrentAnimation(AnimState _state)
        {
            switch (_state)
            {
                case AnimState.IDLE:
                    _AsncAnimation(animClip[(int)AnimState.IDLE], true, 1f);
                    break;
                case AnimState.WALK:
                    _AsncAnimation(animClip[(int)AnimState.WALK], true, 1f);
                    break;
                case AnimState.RUN:
                    _AsncAnimation(animClip[(int)AnimState.RUN], true, 1f);
                    break;
            }
            //짧게 개선한 코드
            //_AsncAnimation(animClip[(int)_state],true, 1f);  

        }

    }
}
