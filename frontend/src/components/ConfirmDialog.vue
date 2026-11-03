<script setup>
defineProps({
  open: { type: Boolean, default: false },
  title: { type: String, required: true },
  message: { type: String, required: true },
  confirmLabel: { type: String, default: 'Confirm' },
  busy: { type: Boolean, default: false }
})

defineEmits(['cancel', 'confirm'])
</script>

<template>
  <Teleport to="body">
    <div
      v-if="open"
      class="fixed inset-0 z-50 flex items-center justify-center bg-ink-950/40 p-4 backdrop-blur-[2px]"
      @click.self="$emit('cancel')"
    >
      <div class="panel w-full max-w-md p-6" role="dialog" aria-modal="true">
        <h3 class="font-display text-xl text-ink-950">{{ title }}</h3>
        <p class="mt-2 text-sm leading-relaxed text-ink-600">{{ message }}</p>
        <div class="mt-6 flex justify-end gap-2">
          <button type="button" class="btn-secondary" :disabled="busy" @click="$emit('cancel')">
            Cancel
          </button>
          <button type="button" class="btn-danger" :disabled="busy" @click="$emit('confirm')">
            {{ busy ? 'Working…' : confirmLabel }}
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>
