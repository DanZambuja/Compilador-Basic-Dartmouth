.global _start, lock, unlock, get_process_id, print_and_draw
.text
_start:
  b _Reset                        @posicao 0x00 - Reset
  ldr pc, _undefined_instruction  @posicao 0x04 - Instrucao nao-definida
  ldr pc, _software_interrupt     @posicao 0x08 - Interrupcao de Software
  ldr pc, _prefetch_abort         @posicao 0x0C - Prefetch Abort
  ldr pc, _data_abort             @posicao 0x10 - Data Abort
  ldr pc, _not_used               @posicao 0x14 - Nao Utilizado
  ldr pc, _irq                    @posicao 0x18 - Interrupcao(IRQ)
  ldr pc, _fiq                    @posicao 0x1C - Interrupcao(FIQ)

_undefined_instruction: .word undefined_instruction
_software_interrupt: .word software_interrupt
_prefetch_abort: .word prefetch_abort
_data_abort: .word data_abort
_not_used: .word not_used
_irq: .word irq
_fiq: .word fiq

INTPND:   .word 0x10140000 @Interrupt status register
INTSEL:   .word 0x1014000C @interrupt select register( 0 = irq, 1 = fiq)
INTEN:    .word 0x10140010 @interrupt enable register
TIMER0L:  .word 0x101E2000 @Timer 0 load register
TIMER0V:  .word 0x101E2004 @Timer 0 value registers
TIMER0C:  .word 0x101E2008 @timer 0 control register
TIMER0X:  .word 0x101E200c @timer 0 interrupt clear register

_Reset:
  ldr sp, =stack_top
  mrs r0, cpsr
  msr cpsr_ctl, #0b11010010 @ modo IRQ
  ldr sp, =irq_stack_top
  msr cpsr, r0              @ volta modo supervisor
  bl timer_init   @Initialize interrupts and timer 0
  bl setup_uart
  msr cpsr, #0b00010000     @Modo usuario
  @msr cpsr, #0b00010011     @Modo supervisor
  bl  main
  b   .

main:
  b   task

undefined_instruction:
  b .

software_interrupt:
  b do_software_interrupt @vai para o handler de interrupcoes de Software

prefetch_abort:
  b .

data_abort:
  b .

not_used:
  b .

irq:
  stmfd sp!, {r0}
  ldr   r0, INTPND                @Carrega o registrador de status de interrupcao
  ldr   r0, [r0]
  tst   r0, #0x0010               @Verifica se é uma interrupcao de timer
  ldmfd sp!, {r0}
  bne   do_irq_interrupt           @vai para o handler de interrupcoes IRQ
  b     irq_uart

irq_uart:
  sub lr, lr, #4
  stmfd sp!, {r0-r12, lr}   @stack r0-r12 and lr
  bl IRQ_handler            @call IRQ_handler() in C
  ldmfd sp!, {r0-r12, pc}^  @return

fiq:
  b .

lock:
  mrs r0, cpsr
  orr r0, r0, #0x80 @set the I bit to disable IRQ
  msr cpsr, r0
  mov pc, lr @return, by ARM C calling convention, the lr=pc-4, i.e. the next instruction

unlock:
  mrs r0, cpsr
  bic r0, r0, #0x80 @clear the I bit to enable IRQ
  msr cpsr, r0
  mov pc, lr

print_and_draw:
  @stmfd sp!, {r0-r12}   @stack r0-r12 and lr
  MOV   r7, #1
  swi   0x0

get_process_id: @Ainda modo usuario
  @stmfd sp!, {r1-r12}   @stack r0-r12 and lr no modo usuario
  MOV   r7, #0   
  swi   0x0

do_software_interrupt:  @Entra no Modo Supervisor
  cmp   r7, #0
  beq   pid_syscall_interrupt
  cmp   r7, #1
  beq   p_d_syscall_interrupt

pid_syscall_interrupt:
  bl  get_pid_syscall
  ldmfd sp!, {pc}^ 
  @msr   cpsr, #0b00010000 @Volta para o modo usuario para desempilhar
  @ldmfd sp!, {r1-r12}
  @mov  pc, lr  

p_d_syscall_interrupt:
  bl  p_d_syscall
  ldmfd sp!, {pc}^ 
  @msr   cpsr, #0b00010000  
  @ldmfd sp!, {r0-r12}  
  @mov  pc, lr

do_irq_interrupt:
  stmfd sp!, {r0, r1, r2, r3, r4, r6, r7} 
  ldr   r0, =nproc
  ldr   r1, [r0]
run_task:
  adr   r7, tabela_proc           @Inicio da tabela_proc
  ldr   r2, =68                   @Espaco na memoria para um processo
  mul   r3, r2, r1                @Obtem index do processo na tabela_proc, r1 = nproc
  add   r4, r7, r3                @Obtem endereco do index do processo na tabela_proc
  add   r6, r1, #1                @Incrementa nproc
  str   r6, [r0]                  @Armazena novo valor de nproc
  ldmfd sp!, {r0}
  str   r0, [r4]                  @Guarda r1 na linha da tabela_proc do processo atual
  add   r4, r4, #4
  mov   r0, r4                    @Guarda a segunda posição da linha da tabela_proc, tem que pegar o endereco do index na tabela + 4 pra salvar o resto dos registradores
  ldmfd sp!, {r1, r2, r3, r4, r6, r7}
continue_do_irq_interrupt:
  sub   lr, lr, #4                @Correcao do LR
  stm   r0!, {r1-r12, lr}         @Guarda os registradores r1-r12 e lr(pc) na linha correspondente de tabela_proc, obs r0 ja foi salvo anteriormente em line 80
  mrs   r14, spsr
  str   r14, [r0], #4             @Guarda spsr
  @and   r2, r14, #31              @Pega os bits do modo no spsr ESSA LINHA DA RUIM
  @cmp   r2, #16                   @Compara com modo usuario
help:
  @ldreq r2, =223                  @Guarda modo system no r2
  @ldrne r2, =211                  @Guarda modo supervisor no r2
  mrs   r1, cpsr
  @cmp   r2, #223
  @msreq cpsr_ctl, #0b11011111     @Entra no modo system
  @msr   cpsr_ctl, #0b11011111     @System mode APAGAR DEPOIS 
  msr    cpsr_ctl, #0b11010011     @Entra no modo supervisor
  stm   r0!, {sp, lr}             @Guarda sp e lr
  msr   cpsr, r1                  @Exit system/supervisor mode
  blne  handler_timer             @vai para a rotina de tratamento da interrupcao de timer
  ldr   r0, =nproc
  ldr   r1, [r0]                  @r1 = valor de nproc
  cmp   r1, #10                   @Verifica o conteudo de nproc
  ldreq r1, =0                    @Reseta r1
  streq r1, [r0]                  @Reseta nproc
  ldr   r5, =68                   @Offset para index da tabela_proc
  mul   r2, r1, r5                @Obtem index do processo na tabela_proc
  adr   r3, tabela_proc
  add   r4, r3, r2                @Soma o index ao endereco de inicio da tabela_proc para obter a pilha do processo
  add   r4, r4, #68               @Offset para topo da stack
  mov   r0, r4
help2:
  mrs   r14, spsr
  @and   r2, r14, #31              @Pega os bits do modo no spsr ESSA LINHA DA RUIM
  @cmp   r2, #16                   @Compara com modo usuario
  @ldreq r2, =223                  @Guarda modo system no r2
  @ldrne r2, =211                  @Guarda modo supervisor no r2
  mrs   r1, cpsr
  @cmp   r2, #223
  @msreq cpsr_ctl, #0b11011111     @Entra no modo system
  @msr   cpsr_ctl, #0b11011111     @System mode APAGAR DEPOIS 
  msr cpsr_ctl, #0b11010011     @Entra no modo supervisor
  ldmdb r0!, {sp, lr}             @Restaura sp e lr
  msr   cpsr, r1                  @Exit system/supervisor mode
  ldr   r14, [r0, #-4]!
  msr   spsr, r14                 @Restaura spsr
  ldmdb r0, {r0-r12, pc}^         @Restaura r1-r12 e pc

timer_init:
  LDR r0, INTEN
  LDR r1,=0x10      @bit 4 for timer 0 interrupt enable
  STR r1,[r0]
  LDR r0, TIMER0L
  LDR r1, =0xfffff @setting timer value
  STR r1,[r0]
  LDR r0, TIMER0C
  MOV r1, #0xE0     @enable timer module
  STR r1, [r0]
  mrs r0, cpsr
  bic r0,r0,#0x80
  msr cpsr_c,r0     @enabling interrupts in the cpsr
  mov pc, lr



@ 0b00010011 - modo supervisor
@ 0b00010000 - modo usuario

tabela_proc:
  .space 52
  .word  task           @label da task1
  .word  0b00010000     @modo usuário com interrupções ativadas
  .word  a_stack_top    @começo da pilha de task1
  .space 4
  .space 52
  .word  task           @label da task2
  .word  0b00010000     @modo usuário com interrupções ativadas
  .word  b_stack_top    @começo da pilha de task2
  .space 4
  .space 52
  .word  task           @label da task3
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  c_stack_top    @começo da pilha de task3
  .space 4
  .space 52
  .word  task           @label da task4
  .word  0b00010000     @modo usuário com interrupções ativadas
  .word  d_stack_top    @começo da pilha de task4
  .space 4
  .space 52
  .word  task           @label da task5
  .word  0b00010000     @modo usuário com interrupções ativadas
  .word  e_stack_top    @começo da pilha de task5
  .space 4
  .space 52
  .word  task           @label da task6
  .word  0b00010000     @modo usuário com interrupções ativadas
  .word  f_stack_top    @começo da pilha de task6
  .space 4
  .space 52
  .word  task           @label da task7
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  g_stack_top    @começo da pilha de task7
  .space 4
  .space 52
  .word  task           @label da task8
  .word  0b00010000     @modo usuário com interrupções ativadas
  .word  h_stack_top    @começo da pilha de task8
  .space 4
  .space 52
  .word  task           @label da task9
  .word  0b00010000     @modo usuário com interrupções ativadas
  .word  i_stack_top    @começo da pilha de task9
  .space 4
  .space 52
  .word  task           @label da task10
  .word  0b00010000     @modo usuário com interrupções ativadas
  .word  j_stack_top    @começo da pilha de task10
  .space 4

tabela_proc_svc:
  .space 52
  .word  task           @label da task1
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  a_stack_top_svc    @começo da pilha de task1
  .space 4
  .space 52
  .word  task           @label da task2
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  b_stack_top_svc    @começo da pilha de task2
  .space 4
  .space 52
  .word  task           @label da task3
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  c_stack_top_svc    @começo da pilha de task3
  .space 4
  .space 52
  .word  task           @label da task4
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  d_stack_top_svc    @começo da pilha de task4
  .space 4
  .space 52
  .word  task           @label da task5
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  e_stack_top_svc    @começo da pilha de task5
  .space 4
  .space 52
  .word  task           @label da task6
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  f_stack_top_svc    @começo da pilha de task6
  .space 4
  .space 52
  .word  task           @label da task7
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  g_stack_top_svc    @começo da pilha de task7
  .space 4
  .space 52
  .word  task           @label da task8
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  h_stack_top_svc    @começo da pilha de task8
  .space 4
  .space 52
  .word  task           @label da task9
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  i_stack_top_svc    @começo da pilha de task9
  .space 4
  .space 52
  .word  task           @label da task10
  .word  0b00010011     @modo usuário com interrupções ativadas
  .word  j_stack_top_svc    @começo da pilha de task10
  .space 4

