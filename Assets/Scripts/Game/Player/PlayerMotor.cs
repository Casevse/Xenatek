﻿using UnityEngine;
    private bool isFrozen = false;
        // Modify the base speeds according to the speed capacity.
        baseWalkSpeed += baseWalkSpeed * XenatekManager.xenatek.speedCapacity * 0.3f;
        baseRunSpeed += baseRunSpeed * XenatekManager.xenatek.speedCapacity * 0.3f;

        if (!isFrozen) {
            inputX = Input.GetAxis("Movement X");
            inputY = Input.GetAxis("Movement Y");

            // Run
            if (Input.GetButton("Run") || Input.GetAxis("Run") > 0.0f) {
                speed = runSpeed;
            }
            else {
                speed = walkSpeed;
            }

            // Jump
            if (controller.isGrounded) {
                moveDirection = new Vector3(inputX, 0.0f, inputY);
                moveDirection = transform.TransformDirection(moveDirection) * speed;
                if (!Input.GetButton("Jump")) {
                    jumpTimer++;
                }
                else if (jumpTimer >= antiBunnyHopFactor) {
                    moveDirection.y = jumpSpeed;
                    jumpTimer = 0;
                }
            }
            else {
                moveDirection.x = inputX * speed;
                moveDirection.z = inputY * speed;
                moveDirection = transform.TransformDirection(moveDirection);
            }
        }
        else {
            moveDirection = Vector3.zero;
            moveDirection.y -= gravity * Time.deltaTime * 4.0f;    // To fall faster.
        }

    // Apply the speed power up. The values are added here directly. + 30 %
    public void SetSpeedPowerUp(bool activate) {
        if (activate) {
            walkSpeed = baseWalkSpeed + baseWalkSpeed * 0.7f;
            runSpeed = baseRunSpeed + baseRunSpeed * 0.7f;
        }
        else {
            walkSpeed = baseWalkSpeed;
            runSpeed = baseRunSpeed;
        }
    }
    public void Freeze(bool isDead) {
        isFrozen = true;
        if (isDead) {
            // If we are dead, simulate the falling to the floor.
            controller.height = 0.1f;
            controller.radius = 0.1f;
        }
    }