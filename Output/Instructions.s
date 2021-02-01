LABEL_10:
   ldr r11, =4
   ldr r12, =25
   add r0, r12, r11
   mov r12, r0
   mov r0, r12
   ldr r1, =0
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
LABEL_20:
   ldr r0, =0
   ldr r1, =1
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
   ldr r0, =6
   ldr r1, =2
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
   ldr r0, =2
   ldr r1, =3
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
LABEL_30:
   ldr r0, =PRINT_1
   bl print_uart0
LABEL_40:
   ldr r1, =1
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   ldr r0, [r2]
   ldr r1, =2
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   ldr r12, [r2]
   ldr r1, =3
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   ldr r11, [r2]
   add r0, r0, r11
   ldr r1, =1
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
   cmp r12, #0
   bge PARADA_POSITIVA_I_MAX_LOOP
   b PARADA_NEGATIVA_I_MAX_LOOP
PARADA_POSITIVA_I_MAX_LOOP:
   cmp r0, r12
   ble LABEL_30
   b FIM_LOOP_I_MAX_LOOP
PARADA_NEGATIVA_I_MAX_LOOP:
   cmp r0, r12
   bge LABEL_30
FIM_LOOP_I_MAX_LOOP:
