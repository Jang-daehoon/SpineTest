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
        //������ �ִϸ��̼��� ���� ��
        public SkeletonAnimation skeletonAnimation;
        public AnimationReferenceAsset[] animClip;

        //�ִϸ��̼ǿ� ���� ENUM
        public enum AnimState
        {
            IDLE, WALK, RUN, ATTACK, JUMP, MORNINGSTARPOS
        }
        #endregion
        #region private Field
        //���� �ִϸ��̼� ó���� ���������� ���� ����
        private AnimState animState;
        private string currentAnimation;    //���� � �ִϸ��̼��� ����ǰ� �ִ����� ���� ����

        //Moveó��
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
            //���ݽ�
            if(Input.GetKeyDown(KeyCode.Space))
            {
                //��Ų ���� (��: "weapon/sword")
                skeletonAnimation.initialSkinName = "weapon/sword";
                skeletonAnimation.Initialize(true); // ��Ų �ʱ�ȭ�� ����
                //ChangeFaceOnly(animClip[(int)AnimState.ATTACK], false, 1f, 1);

                //1ȸ ���� �� ���� �ִϸ��̼��� ����
                ChangeFaceOne(animClip[(int)AnimState.ATTACK], false, 1f, animClip[(int)AnimState.IDLE]);
            }

            //STATE�� ���� �ִϸ��̼� ���� �ִϸ��̼��� ��� �񵿱� ó��
            SetCurrentAnimation(animState);

        }
        private void FixedUpdate()
        {
            rig.velocity = new Vector2 (xx * 300 * Time.deltaTime, rig.velocity.y);
        }

        private void _AsncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)
        {
            if (animClip.name.Equals(currentAnimation)) return; //�� �����Ӹ��� �������� �ʵ��� ����.

            //�ش� �ִϸ��̼����� �����Ѵ�.
            skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
            skeletonAnimation.loop = loop;
            skeletonAnimation.timeScale = timeScale;

            //���� ����ǰ� �ִ� �ִϸ��̼� ���� ����.
            currentAnimation = animClip.name;
        }
        private void ChangeFaceOnly(AnimationReferenceAsset faceAnimClip, bool loop, float timeScale, int track = 1)
        {
            // ������ Ʈ��(1�� Ʈ��)���� �ִϸ��̼� ���
            TrackEntry entry = skeletonAnimation.state.SetAnimation(track, faceAnimClip, loop);
            entry.TimeScale = timeScale;
          
            // ǥ�� �ִϸ��̼��� ����Ǹ� �ش� Ʈ���� ���ų� ����
            if (!loop)
            {
                entry.Complete += (trackEntry) =>
                {
                    skeletonAnimation.state.ClearTrack(track); // ǥ�� �ִϸ��̼��� ������ Ʈ���� ���
                };
            }
        }
        private void ChangeFaceOne(AnimationReferenceAsset animClip, bool loop, float timeScale, AnimationReferenceAsset defaultAnimClip, int track = 0)
        {
            // ������ �ִϸ��̼� ���
            TrackEntry entry = skeletonAnimation.state.SetAnimation(track, animClip, loop);
            entry.TimeScale = timeScale;

            // �ִϸ��̼� ���� �� �⺻ ���·� ����
            entry.Complete += (trackEntry) =>
            {
                skeletonAnimation.state.SetAnimation(track, defaultAnimClip, true).TimeScale = 1f; // �⺻ �ִϸ��̼����� ����
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
            //ª�� ������ �ڵ�
            //_AsncAnimation(animClip[(int)_state],true, 1f);  

        }

    }
}
