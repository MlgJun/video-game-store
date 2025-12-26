<template>
  <button class="checkbox" :aria-pressed="checked.toString()" @click="toggle" type="button">
    <img v-if="checked" src="/src/assets/icons/check.svg" alt="check" />
  </button>
</template>

<script setup>
import { computed } from 'vue'
const props = defineProps({ modelValue: { type: Boolean, default: false } })
const emit = defineEmits(['update:modelValue'])
const checked = computed(() => props.modelValue)
function toggle() { emit('update:modelValue', !props.modelValue) }
</script>

<style scoped>
.checkbox{width:40px;height:40px;background:#222222;border:2px solid #A53DFF;border-radius:10px;display:flex;align-items:center;justify-content:center;padding:0;cursor:pointer}
.checkbox img{width:20px;height:20px;filter:brightness(0) invert(1)}
</style>
