.global _start, lock, unlock
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

_Reset:
  ldr sp, =stack_top
  mrs r0, cpsr
  msr cpsr_ctl, #0b11010010 @ modo IRQ
  ldr sp, =irq_stack_top
  msr cpsr, r0              @ volta modo supervisor
  bl setup_uart
  @msr cpsr, #0b00010000     @Modo usuario
  msr cpsr, #0b00010011     @Modo supervisor
  bl  main
  b   .

undefined_instruction:
  b .

software_interrupt:
  b . 

prefetch_abort:
  b .

data_abort:
  b .

not_used:
  b .

irq:
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

main:
  