LABEL_10:
   ldr r0, =1
   ldr r1, =0
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
   ldr r0, =10
   ldr r1, =1
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
   ldr r0, =1
   ldr r1, =2
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   str r0, [r2]
LABEL_20:
   ldr r0, =PRINT_1
   bl print_uart0
LABEL_30:
   ldr r1, =0
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   ldr r0, [r2]
   ldr r1, =1
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   ldr r12, [r2]
   ldr r1, =2
   adr r2, mem
   ldr r3, =4
   mul r5, r1, r3
   add r2, r2, r5
   ldr r11, [r2]
   add r0, r0, r11
   ldr r1, =0
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
   ble LABEL_20
   b FIM_LOOP_I_MAX_LOOP
PARADA_NEGATIVA_I_MAX_LOOP:
   cmp r0, r12
   bge LABEL_20
FIM_LOOP_I_MAX_LOOP:
